using System;
using Monitis.API.Domain.Monitors;

namespace Monitis.Prototype.Logic.Monitis.MonitorDescriptors
{
    public class TableAvailability : MonitorDescriptor
    {
        public TableAvailability(String storageAccountName)
        {
            Name = String.Format(@"[Account - {0}; {1}]", storageAccountName,
                                 MonitorDesriptorsResource.TableServiceAvailability);
            Type = MonitorDesriptorsResource.DefaultMonitorType;
            Tag = MonitorDesriptorsResource.DefaultMonitorTag;
            ResultParams = new[]
                               {
                                   new ResultParameterDescriptor
                                       {
                                           DataType = MeasureDataType.Integer,
                                           Name = MonitorDesriptorsResource.TableServiceAvailability,
                                           DisplayName = MonitorDesriptorsResource.Value,
                                           UOM = MonitorDesriptorsResource.PercentSign
                                       }
                               };
        }
    }
}