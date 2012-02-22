using System;
using Monitis.API.Common;

namespace Monitis.API.REST.CustomMonitors.Contract
{
    /// <summary>
    /// Response on method call <see cref="CustomMonitorAPI.AddMonitor"/>
    /// </summary>
    public class AddMonitorResponse : APIResponce
    {
        /// <summary>
        /// Contains ID of added monitor
        /// </summary>
        public Int32 Data { get; set; }
    }
}