using System;
using Monitis.API.REST.CustomMonitors;

namespace Monitis.API.Domain.Monitors
{
    /// <summary>
    /// Class contains information about <see cref="Monitor"/> <see cref="ResultParameter"/>
    /// </summary>
    public class ResultParameterDescriptor
    {
        /// <summary>
        /// Name for internal usages
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Name which user see in dashboard
        /// </summary>
        public String DisplayName { get; set; }
        /// <summary>
        ///  Is unit of measure(user defined string parameter, e.g. ms, s, kB, MB, GB, GHz, kbit/s, ... ).
        /// </summary>
        public String UOM { get; set; }

        /// <summary>
        /// Type of data
        /// </summary>
        public MeasureDataType DataType { get; set; }
    }
}