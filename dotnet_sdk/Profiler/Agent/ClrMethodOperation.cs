using System;

namespace AgentCore
{
    public struct ClrMethodOperation
    {
        public ClrMethodInfo MethodInfo;
        public MethodAction Action;
        public DateTime Time;

        public ClrMethodOperation(ClrMethodInfo methodInfo, MethodAction action, DateTime time)
        {
            MethodInfo = methodInfo;
            Action = action;
            Time = time;
        }

        public override string ToString()
        {
            return string.Format("MethodInfo: {0}, Action: {1}, Time: {2}", MethodInfo, Action, Time);
        }
    }

    public enum MethodAction
    {
        Start,
        End
    }

    public struct ClrMethodInfo
    {
        public ClrMethodInfo(string assemblyName, string methodName, string unitOfWork, long threadID)
        {
            AssemblyName = assemblyName;
            MethodName = methodName;
            UnitOfWork = unitOfWork;
            ThreadID = threadID;
        }

        public override string ToString()
        {
            return string.Format("AssemblyName: {0}, MethodName: {1}, UnitOfWork: {2}, Thread: {3}", AssemblyName, MethodName, UnitOfWork, ThreadID);
        }

        public string AssemblyName;
        public string MethodName;
        public string UnitOfWork;
        public long ThreadID;
    }
}
