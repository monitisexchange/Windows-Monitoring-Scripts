using System;
using System.Globalization;
using Monitis.API.Domain.Monitors;

namespace Monitis.Prototype.Logic.Monitis.MonitorDescriptors
{
    /// <summary>
    /// Predifined descriptor for CPU counter in Windows Compute service e.g. single role instance
    /// </summary>
    public class CPU : MonitorDescriptor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CPU()
        {
            Name = MonitorDesriptorsResource.CPUCounterName;
            Type = MonitorDesriptorsResource.DefaultMonitorType;
            Tag = MonitorDesriptorsResource.DefaultMonitorTag;
            ResultParams = new[]
                               {
                                   new ResultParameterDescriptor
                                       {
                                           Name = Resources.MonitisCPUParameter,
                                           DataType = MeasureDataType.Float,
                                           DisplayName = MonitorDesriptorsResource.Value,
                                           UOM = MonitorDesriptorsResource.PercentSign,
                                       }
                               };
            Convert = input => input.ToString(CultureInfo.InvariantCulture);
        }
    }
}