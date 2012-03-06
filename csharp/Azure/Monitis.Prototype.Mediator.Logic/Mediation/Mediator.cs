using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Web;
using Monitis.API.Domain.Monitors;
using Monitis.API.REST.CustomMonitors;
using Monitis.Prototype.Logic.Azure;
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
        /// <summary>
        /// Convert Windows Azure perfomance counter data to Monitis monitor <see cref="ResultParameter"/>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        private static List<ResultParameter> ConvertToMonitorResults(IEnumerable<PerformanceData> data, Func<Double, String> convert)
        {
            List<ResultParameter> monitorData = new List<ResultParameter>();
            foreach (var performanceData in data)
            {
                String monitisParameterName = UserSession.GetMonitisParameterName(performanceData.CounterName);
                ResultParameter parameter = ConvertToMonitorResult(performanceData.CounterValue, convert, monitisParameterName, performanceData.Timestamp);
                monitorData.Add(parameter);
            }
            return monitorData;
        }


        public static ResultParameter ConvertToMonitorResult(Double value, Func<Double, String> convert, String monitisParameterName, DateTime timeStamp)
        {
            String convertedValue = convert == null
                             ? value.ToString()
                             : convert(value);
            return new ResultParameter
            {
                Name = HttpUtility.UrlEncode(monitisParameterName),
                Value = HttpUtility.UrlEncode(convertedValue),
                Timestamp = timeStamp
            };
        }


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
            //create query executer for access to data in Windows Azure Table Service
            QueryExecuter queryExecuter = new QueryExecuter(_userSession.AzureInfo.AccountName, _userSession.AzureInfo.AccountKey);
            MetricsTransactionsEntity[] metricsTransactionsEntities = queryExecuter.GetMetricData(start, end);
            CustomMonitorAPI customMonitorAPI = new CustomMonitorAPI(_userSession.APIKey, _userSession.APIType);

            foreach (var metricsTransactionsEntity in metricsTransactionsEntities)
            {
                CustomMonitorConfig availabilityConfig =
                    CustomMonitorList.Singleton.GetConfigByMetricName(MetricNames.Availability);
                ResultParameter convertToMonitorResult = ConvertToMonitorResult(metricsTransactionsEntity.Availability,
                                                                                availabilityConfig.Descriptor.Convert,
                                                                                "Count",
                                                                                metricsTransactionsEntity.Timestamp);
                customMonitorAPI.AddResults(_userSession.CurrentAuthToken, availabilityConfig.MonitorID,
                                            new List<ResultParameter> {convertToMonitorResult});

                CustomMonitorConfig totalRequestsConfig =
                    CustomMonitorList.Singleton.GetConfigByMetricName(MetricNames.TotalRequests);
                convertToMonitorResult = ConvertToMonitorResult(metricsTransactionsEntity.TotalRequests,
                                                                totalRequestsConfig.Descriptor.Convert, "Count",
                                                                metricsTransactionsEntity.Timestamp);
                customMonitorAPI.AddResults(_userSession.CurrentAuthToken, totalRequestsConfig.MonitorID,
                                            new List<ResultParameter> {convertToMonitorResult});

                CustomMonitorConfig totalBillableRequestsConfig =
                    CustomMonitorList.Singleton.GetConfigByMetricName(MetricNames.TotalBillableRequests);
                convertToMonitorResult = ConvertToMonitorResult(metricsTransactionsEntity.TotalBillableRequests,
                                                                availabilityConfig.Descriptor.Convert, "Count",
                                                                metricsTransactionsEntity.Timestamp);
                customMonitorAPI.AddResults(_userSession.CurrentAuthToken, totalBillableRequestsConfig.MonitorID,
                                            new List<ResultParameter> {convertToMonitorResult});
            }
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
            QueryExecuter queryExecuter = new QueryExecuter(_userSession.AzureInfo.AccountName, _userSession.AzureInfo.AccountKey);

            CustomMonitorAPI customMonitorAPI = new CustomMonitorAPI(_userSession.APIKey, _userSession.APIType);
            foreach (var kvp in _counterNameEventUpdateMap)
            {
                InvokeStatusChanged(kvp.Key + "> start update");

                //get perfomance counter data
                IEnumerable<PerformanceData> data = queryExecuter.GetPerformanceCounters(kvp.Key,
                                                                                   _userSession.AzureInfo.DeploymentInfo.RoleInstanceName,
                                                                                   DateTime.UtcNow.AddSeconds(-_syncPeriod.TotalSeconds),
                                                                                   DateTime.UtcNow);

                InvokeStatusChanged(kvp.Key + "> retrieved records count from Azure:" + data.Count());


                CustomMonitorConfig configByMetricName = CustomMonitorList.Singleton.GetConfigByMetricName(kvp.Key);

                //convert Windows Azure perfomance counters data to monitis monitor parameters data
                List<ResultParameter> convertToMonitorResults = ConvertToMonitorResults(data, configByMetricName.Descriptor.Convert);

                InvokeStatusChanged(kvp.Key + "> push data to Monitis");


                //add result to monitor
                customMonitorAPI.AddResults(_userSession.CurrentAuthToken, configByMetricName.MonitorID, convertToMonitorResults);

                //fire event to update all handlers
                kvp.Value(this, new CounterDataEventArgs { PerformanceDatas = data });
            }

            InvokeStatusChanged(String.Format("Next update after {0} second", TimeSpan.FromMilliseconds(_updateTimer.Interval).TotalSeconds));
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

        #endregion private methods

        #region private fields

        private Timer _updateTimer;
        private readonly TimeSpan _syncPeriod;
        private readonly UserSession _userSession;
        private readonly Dictionary<String, EventHandler<CounterDataEventArgs>> _counterNameEventUpdateMap = new Dictionary<String, EventHandler<CounterDataEventArgs>>();

        #endregion private fields
    }
}