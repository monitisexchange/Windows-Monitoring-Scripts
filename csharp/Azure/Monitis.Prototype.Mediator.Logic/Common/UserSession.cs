using System;
using System.Collections.Generic;
using System.Linq;
using Monitis.API.Common;
using Monitis.API.REST.CustomMonitors;
using Monitis.API.REST.CustomMonitors.Contract;
using Monitis.API.REST.User;
using Monitis.Prototype.Logic.Monitis;
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

        public UserSession(String apiKey, APIType apiType)
        {
            if (String.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException("apiKey");
            }
            APIKey = apiKey;
            APIType = apiType;
        }

        #region public methods

        /// <summary>
        /// Start new session by retrieving new Secret Key and AuthToken from Monitis API.
        /// </summary>
        public ActionResult Start()
        {
            ActionResult actionResult = new ActionResult();

            UserAPI userAPI = new UserAPI(APIKey, APIType);

            _secretKey = userAPI.GetSecretKey();
            if (String.IsNullOrEmpty(_secretKey))
            {
                actionResult.AddError("Can't get secretkey");
            }
            else
            {
                CurrentAuthToken = userAPI.GetAuthToken(_secretKey);
                if (String.IsNullOrEmpty(CurrentAuthToken))
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
        /// Monitis API key
        /// </summary>
        public String APIKey { get; private set; }

        /// <summary>
        /// Currently support onli www.monitis.com API host
        /// </summary>
        public APIType APIType { get; set; }

        /// <summary>
        /// Session depend auth token
        /// </summary>
        public String CurrentAuthToken { get; private set; }

        /// <summary>
        /// Hold information about connecting to Azure
        /// </summary>
        public AzureInfo AzureInfo { get; set; }

        #endregion public properties

        #region public methods

        /// <summary>
        /// Check if all required monitors exists in monitis API
        /// </summary>
        /// <returns></returns>
        public Boolean CheckCustomMonitors()
        {
            CustomMonitorAPI customMonitorAPI = new CustomMonitorAPI(this.APIKey, this.APIType);
            IEnumerable<Monitor> monitorList = customMonitorAPI.GetMonitorList();

            foreach (String monitorName in CustomMonitorList.Singleton.MonitorNames)
            {
                String name = monitorName;
                Monitor monitor = monitorList.FirstOrDefault(f => f.Name.Equals(name));
                if (monitor == null)
                {
                    return false;
                }

                CustomMonitorList.Singleton.GetConfigByMonitorName(monitor.Name).MonitorID = monitor.ID;
            }
            return true;
        }

        /// <summary>
        /// Create monitors for Azure metrics and counters in Monitis service
        /// </summary>
        public List<string> CreateAzureMonitors()
        {
            CustomMonitorAPI customMonitorAPI = new CustomMonitorAPI(this.APIKey, this.APIType);

            List<String> notAddedMonitors = new List<String>();
            IEnumerable<Monitor> monitorList = customMonitorAPI.GetMonitorList();
            foreach (var monitorName in CustomMonitorList.Singleton.MonitorNames.Except(monitorList.Select(f => f.Name)))
            {
                AddMonitorResponse addMonitorResponse = customMonitorAPI.AddMonitor(CurrentAuthToken,
                    CustomMonitorList.Singleton.GetConfigByMonitorName(monitorName).Descriptor);

                if (!addMonitorResponse.IsOk)
                {
                    notAddedMonitors.Add(monitorName);
                }
            }
            return notAddedMonitors;
        }

        #endregion public methods

        #region private fields

        private String _secretKey;

        #endregion private fields
    }
}