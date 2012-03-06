using System;

namespace Monitis.Prototype.Logic.StorageMetrics
{
    public static class MetricNames
    {
        public static string TotalRequests
        {
            get { return "TotalRequests"; }
        }

        public static string TotalBillableRequests
        {
            get { return "TotalBillableRequests"; }
        }

        public static string Availability
        {
            get { return "Availability"; }
        }
    }
}