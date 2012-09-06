using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Text;
using System.IO;

namespace ProfilerLauncher
{
    public interface IMnLogs
    {
        void PutMessage(string inMessage);
    }

    public interface IMnStream
    {
        Int32 ReadInt32();
        byte ReadByte();
        byte[] ReadBytes(int inCount);
    }

    class MnDataBroker : IMnStream
    {
        #region Contstants
        private const int kSizeOfBuffer = 1024;
        #endregion

        #region Fields
        private byte[] _buffer;
        #endregion

        #region Properties
        public NamedPipeServerStream PipeServer {get; private set;}
        public ManualResetEvent HasData { get; private set;}
        public Queue<byte> Queue { get; private set;}
        #endregion

        #region Constructor
        public MnDataBroker(NamedPipeServerStream inPipeServer)
        {
            PipeServer = inPipeServer;
            _buffer = new byte[kSizeOfBuffer];
            Queue = new Queue<byte>();
            HasData = new ManualResetEvent(false);
            ReadPipeData();
        }
        #endregion

        #region Methods
        private void ReadPipeData()
        {
            try
            {
                if (!PipeServer.IsConnected)
                {
                    throw new Exception();
                }
                PipeServer.BeginRead(_buffer, 0, kSizeOfBuffer, new AsyncCallback(ApplyData), this);
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);

            }
        }

        private static void ApplyData(IAsyncResult inResult)
        {
            MnDataBroker theReader = inResult.AsyncState as MnDataBroker;
            int sizeReadData;
            try
            {
                sizeReadData = theReader.PipeServer.EndRead(inResult);
                if (sizeReadData == 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception theException)
            {
                theReader.PipeServer.Disconnect();
                System.Diagnostics.Debug.WriteLine(theException);
                return;
            }

            if (sizeReadData > 0)
            {
                lock (theReader.Queue)
                {
                    for (int i = 0; i < sizeReadData; i++)
                        theReader.Queue.Enqueue(theReader._buffer[i]);
                }
                theReader.HasData.Set();
                theReader.ReadPipeData();
            }
        }

        private byte[] Read(int inCount)
        {
            byte[] theData = new byte[inCount];
            while (true)
            {
                if (Queue.Count >= theData.Length)
                {
                    lock (Queue)
                    {
                        for (int i = 0; i < theData.Length; i++)
                        {
                            theData[i] = Queue.Dequeue();
                        }
                    }
                    break;
                }
                else if (!PipeServer.IsConnected)
                {
                    throw new Exception();
                }
                HasData.Reset();
                HasData.WaitOne(1000);
            }
            return theData;
        }
        #endregion

        #region IMnStream Members

        public int ReadInt32()
        {
            return new BinaryReader(
               new MemoryStream(Read(sizeof(int)))).ReadInt32();
        }

        public byte ReadByte()
        {
            return new BinaryReader(
               new MemoryStream(Read(sizeof(byte)))).ReadByte();
        }

        public byte[] ReadBytes(int inCount)
        {
            return Read(inCount);
        }
        #endregion
    }

    public class MnPipeDispatched : IDisposable
    {
        private const string kPipeName = "7682B383-2874-4422-AA8E-AFAEBE089F15";
        private long _pipeCount = 0;

        private NamedPipeServerStream _pipeServerStream;
        private Thread _threadPipe;
        private IMnLogs _logs;

        private List<MnPipeServer> _pipeServers = new List<MnPipeServer>();

        #region Constructor
        public MnPipeDispatched(IMnLogs logs)
        {
            _logs = logs;
            _pipeServerStream = new NamedPipeServerStream(kPipeName, PipeDirection.InOut, 254, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            _threadPipe = new Thread(ThreadServerPipe);
            _threadPipe.Start(this);
        }
        #endregion

        private static void ThreadServerPipe(Object inParam)
        {
            var mnPipeDispatched = inParam as MnPipeDispatched;
            try
            {
                while (true)
                {
                    IAsyncResult asyncResult = mnPipeDispatched._pipeServerStream.BeginWaitForConnection(null, null);
                    asyncResult.AsyncWaitHandle.WaitOne();
                    mnPipeDispatched._pipeServerStream.EndWaitForConnection(asyncResult);
                    var newPipe = kPipeName + mnPipeDispatched._pipeCount;
                    var newPipeBytes = Encoding.Unicode.GetBytes(newPipe);
                    mnPipeDispatched.StartListenPipe(newPipe);
                    ++mnPipeDispatched._pipeCount;
                    mnPipeDispatched._pipeServerStream.Write(newPipeBytes,0,newPipeBytes.Length);
                    mnPipeDispatched._pipeServerStream.Disconnect();
                }
            }
            catch (ThreadAbortException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private void StartListenPipe(string newPipe)
        {
            var pipeServer = new MnPipeServer(_logs, newPipe);
            _pipeServers.Add(pipeServer);
        }

        public void Dispose()
        {
             Stop();
        }

        public void Stop()
        {
            _threadPipe.Abort();
            _threadPipe.Join();
            foreach (var server in _pipeServers)
            {
                server.Stop();
            }
        }
    }

    public class MnPipeServer
    {
        #region Private Constants
        private const Int32 kMarker = 0x03F703F7;
        #endregion

        #region Private date
        private NamedPipeServerStream _pipeServerStream;
        private Thread _threadPipe;
        private IMnLogs _logs;

        #endregion

        #region Constructor
        public MnPipeServer(IMnLogs logs, string pipeName)
        {
            _logs = logs;
            _pipeServerStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 254, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            _threadPipe = new Thread(ThreadServerPipe);
            _threadPipe.Start(this);
        }
        #endregion

        public void Stop()
        {
            _threadPipe.Abort();
            _threadPipe.Join();
        }

        private static void ThreadServerPipe(Object inParam)
        {
            MnPipeServer pipeServer = inParam as MnPipeServer;
            pipeServer._logs.PutMessage("Start thread");
            try
            {
                while (true)
                {
                    IAsyncResult asyncResult = pipeServer._pipeServerStream.BeginWaitForConnection(null, null);
                    asyncResult.AsyncWaitHandle.WaitOne();
                    pipeServer._pipeServerStream.EndWaitForConnection(asyncResult);
                    MnDataBroker dataBroker = new MnDataBroker(pipeServer._pipeServerStream);
                    while (pipeServer._pipeServerStream.IsConnected || dataBroker.Queue.Count > 0)
                    {
                        try
                        {
                            Int32 marker = dataBroker.ReadInt32();
                            while (marker != kMarker)
                            {
                                byte sign = dataBroker.ReadByte();
                                marker = (marker << 8) | sign;
                            }

                            Int32 sizeParams = dataBroker.ReadInt32();
                            byte[] bufParams = dataBroker.ReadBytes(sizeParams);
                            pipeServer._logs.PutMessage(Encoding.Unicode.GetString(bufParams, 0, sizeParams));
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}
