using System;
using System.Collections.Generic;
using Monitis.Prototype.Logic.Azure;
using Monitis.Prototype.Logic.Azure.TableService;

namespace Monitis.Prototype.Logic.Mediation
{
    /// <summary>
    /// Class represents event args for counter data
    /// </summary>
    public class CounterDataEventArgs : EventArgs
    {
        public List<PerformanceData> PerformanceDatas { get; set; }
    }
}