using System;
using System.Threading;
using ProfilerLauncher;

namespace AgentCore
{
    public class Agent : IDisposable
    {
        private readonly Timer _timer;
        private readonly MnPipeDispatched _pipeDispatcher;
        private readonly ClrActionsProcessor _actionsProcessor;
        private readonly IInfoAnalizer _infoAnalizer;
        private const int DelayInMs = 60000;

        public Agent(IInfoAnalizer infoAnalizer, ClrActionsProcessor iLogger)
        {
            _timer = new Timer(ThreadCallback, null, DelayInMs, DelayInMs);
            _actionsProcessor = iLogger;
            _pipeDispatcher = new MnPipeDispatched(_actionsProcessor);
            _infoAnalizer = infoAnalizer;
        }

        public void Stop()
        {
            _timer.Dispose();
            _pipeDispatcher.Stop();
        }

        private void ThreadCallback(object state)
        {
            _infoAnalizer.Analize(_actionsProcessor.PopActions(), DelayInMs);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
