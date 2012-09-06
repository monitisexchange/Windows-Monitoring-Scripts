using System;

namespace AgentCore
{
    public class MessageRecievedEventArgs
        : EventArgs
    {
        public string Message { get; set; }
    }
}