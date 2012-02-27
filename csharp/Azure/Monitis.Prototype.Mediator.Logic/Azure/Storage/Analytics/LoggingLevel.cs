using System;

namespace Monitis.Prototype.Logic.Azure.Storage.Analytics
{
    [Flags]
    public enum LoggingLevel
    {
        None = 0,
        Delete = 2,
        Write = 4,
        Read = 8,
    }
}