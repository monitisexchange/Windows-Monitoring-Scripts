using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Web;
using Monitis.API.Domain.Monitors;
using Monitis.API.REST.CustomMonitors;
using Monitis.Prototype.Logic.Azure.Storage.Tables.MetricsTransactions;
using Monitis.Prototype.Logic.Azure.TableService;
using Monitis.Prototype.Logic.Common;
using Monitis.Prototype.Logic.Monitis;
using Monitis.Prototype.Logic.PerfomanceCounter;
using Monitis.Prototype.Logic.StorageMetrics;
using Timer = System.Timers.Timer;

namespace Monitis.Prototype.Logic.Mediation
{
    /// <summary>
    /// Represents mediation process between Windows Azure and Monitis API
    /// </summary>
    public class Mediator
    {
        #region events

        /// <summary>
        /// Occurs when mediator get new CPU perfomance counter data
        /// </summary>
        public event EventHandler<CounterDataEventArgs> CPUDataUpdated;

        /// <summary>
        /// Occurs when mediator get new Memory perfomance counter data
        /// </summary>
        public event EventHandler<CounterDataEventArgs> MemoryUpdated;

        /// <summary>
        /// Occurs when mediator change status
        /// </summary>
        public event EventHandler<StatusEventArgs> StatusChanged;

        /// <summary>
        /// Occurs when mediator change status for storage service metrics
        /// </summary>
        public event EventHandler<StatusEventArgs> StatusStorageMetricsChanged;

        /// <summary>
        /// Occurs when mediator complete publish storage service metrics data to monitis
        /// </summary>
        public event EventHandler StorageMetricsPublishCompleted;

       

        #endregion events

        #region constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="syncPeriod">Timespan period for sync data between Windows Azure and Monitis API</param>
        /// <param name="userSession">Current user session object</param>
        public Mediator(TimeSpan syncPeriod, UserSession userSession)
        {
            if (userSession == null)
            {
                throw new ArgumentNullException("userSession");
            }
            _syncPeriod = syncPeriod;
            _userSession = userSession;

            //TODO: need separate monitor for CPU and each counter
            _counterNameEventUpdateMap.Add(CounterNames.ProcessorTotalTime, InvokeCpuDataUpdated);
            _counterNameEventUpdateMap.Add(CounterNames.MemoryAvailableBytes, InvokeMemoryUpdated);
        }

        #endregion constructor

        #region public methods

        public void UpdateTableServiceMetrics(DateTime start, DateTime end)
        {
            Thread thread = new Thread(() => MediateTableMetrics(start, end));
            thread.Start();
        }

        

        /// <summary>
        /// Start mediation process for live metrics and counters
        /// </summary>
        public void Start()
        {
            //create and schedule timer for periodic sync process
            _updateTimer = new Timer { Interval = _syncPeriod.TotalMilliseconds, AutoReset = true };
            _updateTimer.Elapsed += UpdatePerfomanceCounters;
            _updateTimer.Start();
            UpdatePerfomanceCounters(this, null);
        }

        /// <summary>
        /// Stop mediation process
        /// </summary>
        public void Stop()
        {
            _updateTimer.Stop();
            _updateTimer.Close();
        }

        #endregion public methods

        #region private methods

        private void MediateTableMetrics(DateTime start, DateTime end)
        {
            var queryExecuter = new QueryExecuter(_userSession.AzureInfo.AccountName, _userSession.AzureInfo.AccountKey);

            InvokeStatusStorageMetricsChanged("Get metrics data from Azure");
            MetricsTransactionsEntity[] metricsTransactionsEntities = queryExecuter.GetMetricData(start, end);

            String statusFormat = "Captured records count: {0}\r\nPushed to monitis service:{1}";

            Int32 totalProcessedRecords = 0;
            InvokeStatusStorageMetricsChanged(String.Format(statusFormat, metricsTransactionsEntities.Length, totalProcessedRecords));
            var customMonitorAPI = new CustomMonitorAPI(_userSession.APIKey, _userSession.APIType);

            foreach (MetricsTransactionsEntity metricsTransactionsEntity in metricsTransactionsEntities)
            {
                CustomMonitorConfig availabilityConfig =
                    CustomMonitorList.Singleton.GetConfigByMetricName(MetricNames.Availability);
                ResultParameter convertToMonitorResult = DataConverter.ConvertToMonitorResult(metricsTransactionsEntity.Availability,
                                                                                metricsTransactionsEntity.Timestamp,
                                                                                availabilityConfig);

                customMonitorAPI.AddResults(_userSession.CurrentAuthToken, availabilityConfig.MonitorID,
                                            new List<ResultParameter> { convertToMonitorResult });

                CustomMonitorConfig totalRequestsConfig =
                    CustomMonitorList.Singleton.GetConfigByMetricName(MetricNames.TotalRequests);
                convertToMonitorResult = DataConverter.ConvertToMonitorResult(metricsTransactionsEntity.TotalRequests,
                                                                metricsTransactionsEntity.Timestamp,
                                                                totalRequestsConfig);
                customMonitorAPI.AddResults(_userSession.CurrentAuthToken, totalRequestsConfig.MonitorID,
                                            new List<ResultParameter> { convertToMonitorResult });

                CustomMonitorConfig totalBillableRequestsConfig =
                    CustomMonitorList.Singleton.GetConfigByMetricName(MetricNames.TotalBillableRequests);
                convertToMonitorResult = DataConverter.ConvertToMonitorResult(metricsTransactionsEntity.TotalBillableRequests,
                                                                metricsTransactionsEntity.Timestamp,
                                                                totalBillableRequestsConfig);
                customMonitorAPI.AddResults(_userSession.CurrentAuthToken, totalBillableRequestsConfig.MonitorID,
                                            new List<ResultParameter> { convertToMonitorResult });

                totalProcessedRecords++;
                InvokeStatusStorageMetricsChanged(String.Format(statusFormat, metricsTransactionsEntities.Length, totalProcessedRecords));
            }
            InvokeStatusStorageMetricsChanged("Completed");
            InvokeStorageMetricsPublishCompleted(EventArgs.Empty);
        }

        /// <summary>
        /// Update monitis custom monitor with new perfomance counters data.
        /// Trigger by timer instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdatePerfomanceCounters(object sender, ElapsedEventArgs e)
        {
            //TODO: look like template pattern

            //create query executer for access to data in Windows Azure Table Service
            var queryExecuter = new QueryExecuter(_userSession.AzureInfo.AccountName, _userSession.AzureInfo.AccountKey);

            var customMonitorAPI = new CustomMonitorAPI(_userSession.APIKey, _userSession.APIType);
            foreach (var kvp in _counterNameEventUpdateMap)
            {
                InvokeStatusChanged(kvp.Key + "> start update");

                //get perfomance counter data
                IEnumerable<PerformanceData> data = queryExecuter.GetPerformanceCounters(kvp.Key,
                                                                                         _userSession.AzureInfo.
                                                                                             DeploymentInfo.
                                                                                             RoleInstanceName,
                                                                                         DateTime.UtcNow.AddSeconds(
                                                                                             -_syncPeriod.TotalSeconds),
                                                                                         DateTime.UtcNow);

                InvokeStatusChanged(kvp.Key + "> retrieved records count from Azure:" + data.Count());


                CustomMonitorConfig configByMetricName = CustomMonitorList.Singleton.GetConfigByMetricName(kvp.Key);

                //convert Windows Azure perfomance counters data to monitis monitor parameters data
                List<ResultParameter> convertToMonitorResults = DataConverter.ConvertToMonitorResults(data,
                                                                                        configByMetricName.Descriptor.
                                                                                            Converter);

                InvokeStatusChanged(kvp.Key + "> push data to Monitis");


                //add result to monitor
                customMonitorAPI.AddResults(_userSession.CurrentAuthToken, configByMetricName.MonitorID,
                                            convertToMonitorResults);

                //fire event to update all handlers
                kvp.Value(this, new CounterDataEventArgs { PerformanceDatas = data });
            }

            InvokeStatusChanged(String.Format("Next update after {0} second",
                                              TimeSpan.FromMilliseconds(_updateTimer.Interval).TotalSeconds));
        }

        private void InvokeStatusChanged(String status)
        {
            EventHandler<StatusEventArgs> handler = StatusChanged;
            if (handler != null) handler(this, new StatusEventArgs { Status = status });
        }

        private void InvokeMemoryUpdated(object sender, CounterDataEventArgs counterDataEventArgs)
        {
            EventHandler<CounterDataEventArgs> handler = MemoryUpdated;
            if (handler != null) handler(sender, counterDataEventArgs);
        }

        private void InvokeCpuDataUpdated(object sender, CounterDataEventArgs counterDataEventArgs)
        {
            EventHandler<CounterDataEventArgs> handler = CPUDataUpdated;
            if (handler != null) handler(sender, counterDataEventArgs);
        }

        private void InvokeStorageMetricsPublishCompleted(EventArgs e)
        {
            EventHandler handler = StorageMetricsPublishCompleted;
            if (handler != null) handler(this, e);
        }

        private void InvokeStatusStorageMetricsChanged(String status)
        {
            EventHandler<StatusEventArgs> handler = StatusStorageMetricsChanged;
            if (handler != null)
            {
                handler(this, new StatusEventArgs { Status = status });
            }
        }

        #endregion private methods

        #region private fields

        private readonly Dictionary<String, EventHandler<CounterDataEventArgs>> _counterNameEventUpdateMap =
            new Dictionary<String, EventHandler<CounterDataEventArgs>>();

        private readonly TimeSpan _syncPeriod;
        private readonly UserSession _userSession;
        private Timer _updateTimer;

        #endregion private fields
    }
}