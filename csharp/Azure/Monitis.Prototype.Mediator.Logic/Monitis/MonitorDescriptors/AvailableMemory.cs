using System;
using System.Globalization;
using Monitis.API.Domain.Monitors;

namespace Monitis.Prototype.Logic.Monitis.MonitorDescriptors
{
    /// <summary>
    /// Predifined descriptor for availble memory counter in Windows Compute service e.g. single role instance
    /// </summary>
    public class AvailableMemory : MonitorDescriptor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AvailableMemory()
        {
            Name = MonitorDesriptorsResource.MemoryCounterName;
            Type = MonitorDesriptorsResource.DefaultMonitorType;
            Tag = MonitorDesriptorsResource.DefaultMonitorTag;
            ResultParams = new[]
                               {
                                   new ResultParameterDescriptor
                                       {
                                           DataType = MeasureDataType.Integer,
                                           DisplayName = MonitorDesriptorsResource.Value,
                                           Name = Resources.MonitisFreeMemoryParameter,
                                           UOM = MonitorDesriptorsResource.MegaBytes
                                       }
                               };
            Convert = input => (input / 1048576).ToString(CultureInfo.InvariantCulture);
        }
    }
}