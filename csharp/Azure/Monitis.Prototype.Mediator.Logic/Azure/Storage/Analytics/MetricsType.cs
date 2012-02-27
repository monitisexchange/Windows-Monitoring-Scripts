using System;

namespace Monitis.Prototype.Logic.Azure.Storage.Analytics
{
    [Flags]
    public enum MetricsType
    {
        None = 0x0,
        ServiceSummary = 0x1,
        ApiSummary = 0x2,
        All = ServiceSummary | ApiSummary,
    }
}