using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ProfilerLauncher;

namespace AgentCore
{
    public class ClrActionsProcessor : IMnLogs
    {
        private readonly BlockingCollection<ClrMethodOperation> _performedActions;
        private readonly ClrMethodMessageParser _parser;

        public ClrActionsProcessor()
        {
            _performedActions = new BlockingCollection<ClrMethodOperation>();
            _parser = new ClrMethodMessageParser();
        }

        public event EventHandler<MessageRecievedEventArgs> MessageRecievedEvent;

        public void OnMessageRecievedEvent(MessageRecievedEventArgs e)
        {
            EventHandler<MessageRecievedEventArgs> handler = MessageRecievedEvent;
            if (handler != null) 
                handler(this, e);
        }

        public void PutMessage(string inMessage)
        {
            Debug.WriteLine(inMessage);
            if (inMessage != "Start thread" && inMessage != "End thread")
            foreach (var operation in _parser.ParseMessage(inMessage.Remove(inMessage.Length-1)))
            {
                _performedActions.Add(operation);
            }


            OnMessageRecievedEvent(new MessageRecievedEventArgs(){Message = inMessage});
        }

        public IEnumerable<ClrMethodOperation> PopActions()
        {
            ClrMethodOperation outer;
            var operations = new List<ClrMethodOperation>();
            while (_performedActions.TryTake(out outer))
                operations.Add(outer);
            return operations;
        }
    }

    public class ClrMethodMessageParser : IParser<IEnumerable<ClrMethodOperation>>
    {
        private readonly char[] separatorEn = new char[]{';'};
        private readonly char[] separatorEq = new char[] { '=' };
        private readonly char[] separatorComma = new char[] { ',' };
        private readonly char[] separatorQuotes = new char[] { '"' };

        public IEnumerable<ClrMethodOperation> ParseMessage(string message)
        {
            var parts = message.Split(separatorEn);
            DateTime? time = null;
            MethodAction? action = null;
            long? managedThreadID = null;
            string methodName = null;
            string assemblyInfo = null;
            IEnumerable<string> units = new string[0];
            for (int i = 0; i < parts.Length; i++)
            {
                var property = parts[i].Split(separatorEq);
                switch (property[0])
                {
                    case "DateTime":
                        time = DateTime.Parse(property[1].Trim(separatorQuotes));
                        break;
                    case "command":
                        action = property[1] == "start" ? MethodAction.Start : MethodAction.End;
                        break;
                    case "MethodName":
                        methodName = property[1];
                        break;
                    case "Assembly":
                        assemblyInfo = property[1];
                        break;
                    case "ManagedThreadId":
                        managedThreadID = int.Parse(property[1]);
                        break;
                    case "UnitProfiles":
                        units = property[1].Split(separatorComma);
                        break;
                }
            }

            if (time == null || action == null || methodName == null || assemblyInfo == null || !units.Any() || managedThreadID == null)
                throw new InvalidOperationException("Can't parse message");

            return units.Select(unit => new ClrMethodOperation(new ClrMethodInfo(assemblyInfo, methodName, unit, managedThreadID.Value), action.Value, time.Value));
        }
    }

    public interface IParser<out T>
    {
        T ParseMessage(string message);
    }
}
