using System;
using Monitis.API.Domain.Monitors;

namespace Monitis.Prototype.Logic.Monitis.MonitorDescriptors
{
    public class TableTotalBillableRequests : MonitorDescriptor
    {
        public TableTotalBillableRequests(String storageAccountName)
        {
            Name = String.Format(@"[Account - {0}; {1}]", storageAccountName,
                              MonitorDesriptorsResource.TableServiceTotalBillableRequests);
            Type = MonitorDesriptorsResource.DefaultMonitorType;
            Tag = MonitorDesriptorsResource.DefaultMonitorTag;
            ResultParams = new[]
                               {
                                   new ResultParameterDescriptor
                                       {
                                           DataType = MeasureDataType.Integer,
                                           Name = MonitorDesriptorsResource.TableServiceTotalBillableRequests,
                                           DisplayName = MonitorDesriptorsResource.Value,
                                           UOM = MonitorDesriptorsResource.PerHour
                                       }
                               };
        }


    }
}