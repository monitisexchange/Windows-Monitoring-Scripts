using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using AgentCore;
using Monitis;

//using System.Windows.Threading;

namespace ProfilerLauncher
{
//     public class MnPipeServer
//     {
//         #region Involve WinAPI function
//         [DllImport("kernel32", EntryPoint = "CreateFile", SetLastError = true)]
//         private static extern SafeFileHandle CreateFile(string inFileName,
//             FileAccess inAccess, FileShare inShare, IntPtr inSecurityAttributes,
//             FileMode inCreationDisposition, FileOptions inFlagsAndAttributes, IntPtr hTemplateFile);
// 
//         [DllImport("kernel32.dll", EntryPoint = "ReadFile", SetLastError = true)]
//         private static extern bool ReadFile(SafeFileHandle hFile, [Out] byte[] lpBuffer,
//             uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, [In] ref NativeOverlapped inOverlapped);
// 
//         [DllImport("kernel32.dll", EntryPoint = "GetOverlappedResult", SetLastError = true)]
//         private static extern bool GetOverlappedResult(SafeFileHandle hFile, [In] ref System.Threading.NativeOverlapped lpOverlapped,
//            out uint lpNumberOfBytesTransferred, bool bWait);
// 
//     //    [DllImport("kernel32.dll", EntryPoint = "CreateNamedPipe", SetLastError = true)]
//     //    private static extern SafeFileHandle CreateNamedPipe(string PipeName, 
//     //__in     DWORD dwOpenMode,
//     //__in     DWORD dwPipeMode,
//     //__in     DWORD nMaxInstances,
//     //__in     DWORD nOutBufferSize,
//     //__in     DWORD nInBufferSize,
//     //__in     DWORD nDefaultTimeOut,
//     //__in_opt LPSECURITY_ATTRIBUTES lpSecurityAttributes
//     );
//         #endregion
// 
//         
// 
//         private SafeFileHandle pipeHandle;
//         public MnPipeServer()
//         {
// 
//             //pipeHandle = CreateFile(kPipeName, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileOptions.Asynchronous, IntPtr.Zero);
//             //if (pipeHandle.IsInvalid)
//             //{
//             //    throw new Win32Exception(Marshal.GetLastWin32Error());
//             //}
//         }
//     }

    public partial class Form1 : Form, IMnLogs
    {
        private const string PROFILER_GUID = "{71EDB19D-4F69-4A2C-A2F5-BE783F543A7E}";
        public static string Login;
        public static string Password;
        private Agent _agent;

        public Form1()
        {
            InitializeComponent();

            this.Closed += Form1Closed;
            this.Load += new EventHandler(Form1_Load);
        }

        private void InitMonitisProfiler()
        {
            var a = new Authentication(Login, Password, null);
            var mon = new MonitisClrMethodInfoAnalizer(a);
            var processor = new ClrActionsProcessor();
            processor.MessageRecievedEvent += processor_MessageRecievedEvent;
            _agent = new Agent(mon, processor);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            var s = new LoginForm();
            if (s.ShowDialog() != DialogResult.Yes)
            {
                Application.Exit();
                return;
            }
            Login = s.Login;
            Password = s.Password;
            InitMonitisProfiler();

            textBox1.Text =Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                         "TestProfilerApplication.exe"));
        }

        void processor_MessageRecievedEvent(object sender, MessageRecievedEventArgs e)
        {
            BeginInvoke(new AddListItem(AddLog), e.Message);
        }

        void Form1Closed(object sender, EventArgs e)
        {
            _agent.Dispose();
        }

        #region IMnLogs Members

        public void PutMessage(string inMessage)
        {
            Invoke(new AddListItem(AddLog), inMessage);
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi;

            // make sure the executable exists
            var path = textBox1.Text.Trim(' ', '\t','\"');
            if (File.Exists(path) == false)
            {
                MessageBox.Show("The executable '" + path +
                                "' does not exist.");
                return;
            }

            // create a process executor
            psi = new ProcessStartInfo(path, textBox2.Text);

            if (psi.EnvironmentVariables.ContainsKey("COR_ENABLE_PROFILING"))
                psi.EnvironmentVariables["COR_ENABLE_PROFILING"] = "1";
            else
                psi.EnvironmentVariables.Add("COR_ENABLE_PROFILING", "1");

            // set the COR_PROFILER env var. This indicates to the CLR which COM object to
            // instantiate for profiling.
            if (psi.EnvironmentVariables.ContainsKey("COR_PROFILER"))
                psi.EnvironmentVariables["COR_PROFILER"] = PROFILER_GUID;
            else
                psi.EnvironmentVariables.Add("COR_PROFILER", PROFILER_GUID);

            psi.UseShellExecute = false;
            psi.WorkingDirectory = Path.GetDirectoryName(path);
            try
            {
                Process p = Process.Start(psi);
            }
            catch (Exception)
            {
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void AddLog(string inMessage)
        {
            logView.Items.Add(inMessage);
        }

        private void tmiClear_Click(object sender, EventArgs e)
        {
            logView.Items.Clear();
        }

        #region Nested type: AddListItem

        private delegate void AddListItem(string inMessage);

        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            var a = new OpenFileDialog();
            a.Multiselect = false;
            a.Filter = "Executable (*.exe)|*.exe";
            if (a.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = a.FileName;
            }
        }
    }
}