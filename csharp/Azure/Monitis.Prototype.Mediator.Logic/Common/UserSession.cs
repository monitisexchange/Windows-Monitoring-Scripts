using System;
using System.Collections.Generic;
using System.Linq;
using Monitis.API.Domain.Monitors;
using Monitis.API.REST.CustomMonitors;
using Monitis.API.REST.CustomMonitors.Contract;
using Monitis.API.REST.User;
using Monitis.Prototype.Logic.PerfomanceCounter;
using Monitor = Monitis.API.Domain.Monitors.Monitor;

namespace Monitis.Prototype.Logic.Common
{
    /// <summary>
    /// Represents user session object for current API, Secret keys and AuthToken
    /// </summary>
    public class UserSession
    {
        /// <summary>
        /// Return mapped name of Monitis parameter for Azure parameter
        /// </summary>
        /// <param name="counterName"></param>
        /// <returns></returns>
        public static String GetMonitisParameterName(String counterName)
        {
            if (counterName == Resources.ProcessorTimeCounter)
                return Resources.MonitisCPUParameter;
            if (counterName == Resources.MemoryAvailableBytes)
                return Resources.MonitisFreeMemoryParameter;
            throw new NotSupportedException("Unknow parameter name");
        }

        public UserSession(String apiKey)
        {
            if (String.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException("apiKey");
            }
            APIKey = apiKey;
        }

        #region public methods

        /// <summary>
        /// Start new session by retrieving new Secret Key and AuthToken from Monitis API.
        /// If
        /// </summary>
        public ActionResult Start()
        {
            ActionResult actionResult = new ActionResult();
            UserAPI userAPI = new UserAPI();
            _secretKey = userAPI.GetSecretKey(APIKey);
            if (String.IsNullOrEmpty(_secretKey))
            {
                actionResult.AddError("Can't get secretkey");
            }
            else
            {
                _authToken = userAPI.GetAuthToken(APIKey, _secretKey);
                if (String.IsNullOrEmpty(_authToken))
                {
                    actionResult.AddError("Can't get authToken");
                }
                else
                {
                    actionResult.IsSuccessful = true;
                }
            }
            return actionResult;
        }

        #endregion public methods

        #region public properties

        /// <summary>
        /// Gets or sets custom active monitor for mediation process
        /// </summary>
        public Monitor CustomActiveMonitor
        {
            get { return _customActiveMonitor; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _customActiveMonitor = value;
            }
        }

        /// <summary>
        /// Monitis API key
        /// </summary>
        public String APIKey { get; private set; }

        public String CurrentAuthToken
        {
            get { return _authToken; }
            set { _authToken = value; }
        }

        /// <summary>
        /// Hold information about connecting to Azure
        /// </summary>
        public AzureInfo AzureInfo { get; set; }

        /// <summary>
        /// Represents standart Monitis monitor for Windows Azure counters
        /// </summary>
        public MonitorConfiguration AzureMonitor
        {
            get
            {
                if (_azureMonitor == null)
                {
                    _azureMonitor = new MonitorConfiguration()
                    {
                        Name = "Windows Azure Monitor",
                        Tag = "Azure",
                        Type = "test",
                        ResultParams = new[]
                        {
                            new ResultParameterDescription
                            { Name = Resources.MonitisCPUParameter, DisplayName = "CPU instance Counter", DataType = MeasureDataType.Float, UOM = "Percent" },
                            new ResultParameterDescription
                            { Name = Resources.MonitisFreeMemoryParameter, DisplayName = "Free instance memory", DataType = MeasureDataType.Integer, UOM = "Bytes" }
                        }
                    };
                }
                return _azureMonitor;
            }
        }

        #endregion public properties

        #region public methods

        /// <summary>
        /// Return list of all custom monitors
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Monitor> GetCustomMonitors()
        {
            CustomMonitorAPI customMonitorAPI = new CustomMonitorAPI();
            IEnumerable<Monitor> monitorList = customMonitorAPI.GetMonitorList(APIKey);
            return monitorList;
        }

        /// <summary>
        /// Create new Azure monitor to Monitis service
        /// </summary>
        public void CreateAzureMonitor()
        {
            CustomMonitorAPI customMonitorAPI = new CustomMonitorAPI();
            Monitor monitor = GetCustomMonitors().FirstOrDefault(f => f.Name.Equals(AzureMonitor.Name));
            if (monitor == null)
            {
                AddMonitorResponse addMonitorResponse = customMonitorAPI.AddMonitor(APIKey, _authToken, AzureMonitor);
            }
            //TODO: need case if monitor exists
        }

        #endregion public methods

        #region private fields

        private MonitorConfiguration _azureMonitor;
        private String _secretKey;
        private String _authToken;
        private Monitor _customActiveMonitor;

        #endregion private fields
    }
}