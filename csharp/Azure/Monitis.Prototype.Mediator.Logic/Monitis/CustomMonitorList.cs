using System;
using System.Collections.Generic;
using System.Linq;
using Monitis.Prototype.Logic.Monitis.MonitorDescriptors;
using Monitis.Prototype.Logic.PerfomanceCounter;
using Monitis.Prototype.Logic.StorageMetrics;

namespace Monitis.Prototype.Logic.Monitis
{
    /// <summary>
    /// Represents list of custom monitors
    /// </summary>
    public class CustomMonitorList
    {
        private static readonly CustomMonitorList _singleton = new CustomMonitorList();

        public static CustomMonitorList Singleton
        {
            get { return _singleton; }
        }

        private CustomMonitorList()
        {
            CustomMonitorConfig cpuConfig = new CustomMonitorConfig(new CPU());
            cpuConfig.SourceMetricName = CounterNames.ProcessorTotalTime;
            _monitorNameConfigMap.Add(cpuConfig.Name, cpuConfig);

            CustomMonitorConfig availableMemoryConfig = new CustomMonitorConfig(new AvailableMemory());
            availableMemoryConfig.SourceMetricName = CounterNames.MemoryAvailableBytes;
            _monitorNameConfigMap.Add(availableMemoryConfig.Name, availableMemoryConfig);

            CustomMonitorConfig storageAccountTotalRequestsConfig = new CustomMonitorConfig(new TableTotalRequests(Resources.DefaultStorageAccountName));
            storageAccountTotalRequestsConfig.SourceMetricName = MetricNames.TotalRequests;
            _monitorNameConfigMap.Add(storageAccountTotalRequestsConfig.Name, storageAccountTotalRequestsConfig);

            CustomMonitorConfig storageAccountTotalBillableRequestsConfig = new CustomMonitorConfig(new TableTotalBillableRequests(Resources.DefaultStorageAccountName));
            storageAccountTotalBillableRequestsConfig.SourceMetricName = MetricNames.TotalBillableRequests;
            _monitorNameConfigMap.Add(storageAccountTotalBillableRequestsConfig.Name, storageAccountTotalBillableRequestsConfig);

            CustomMonitorConfig storageAccountTableAvailabilityConfig = new CustomMonitorConfig(new TableAvailability(Resources.DefaultStorageAccountName));
            storageAccountTableAvailabilityConfig.SourceMetricName = MetricNames.Availability;
            _monitorNameConfigMap.Add(storageAccountTableAvailabilityConfig.Name, storageAccountTableAvailabilityConfig);
        }

        public Boolean Contains(String monitorName)
        {
            return _monitorNameConfigMap.ContainsKey(monitorName);
        }

        public CustomMonitorConfig GetConfigByMonitorName(String monitorName)
        {
            return _monitorNameConfigMap[monitorName];
        }

        public CustomMonitorConfig GetConfigByMetricName(String metricName)
        {
            return _monitorNameConfigMap.Values.Where(f => f.SourceMetricName.Equals(metricName)).First();
        }



        public IEnumerable<String> MonitorNames
        {
            get { return _monitorNameConfigMap.Keys; }
        }


        private readonly Dictionary<String, CustomMonitorConfig> _monitorNameConfigMap = new Dictionary<String, CustomMonitorConfig>();
    }
}