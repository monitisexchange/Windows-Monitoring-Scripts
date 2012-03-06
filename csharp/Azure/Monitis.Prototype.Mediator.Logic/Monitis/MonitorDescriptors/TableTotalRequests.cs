using System;
using Monitis.API.Domain.Monitors;

namespace Monitis.Prototype.Logic.Monitis.MonitorDescriptors
{
    public class TableTotalRequests : MonitorDescriptor
    {
        public TableTotalRequests(String storageAccountName)
        {
            Name = String.Format(@"[Account - {0}; {1}]", storageAccountName,
                                 MonitorDesriptorsResource.TableServiceTotalRequests);
            Type = MonitorDesriptorsResource.DefaultMonitorType;
            Tag = MonitorDesriptorsResource.DefaultMonitorTag;
            ResultParams = new[]
                               {
                                   new ResultParameterDescriptor
                                       {
                                           DataType = MeasureDataType.Integer,
                                           
                                           Name = MonitorDesriptorsResource.Count
                                           //DisplayName = MonitorDesriptorsResource.Bytes,
                                          // UOM = MonitorDesriptorsResource.Bytes
                                       }
                               };
        }
    }
}