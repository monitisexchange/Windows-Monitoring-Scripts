using System;
using System.Collections.Generic;
using Monitis.API.Common;
using Monitis.API.Domain.Monitors;
using Monitis.API.REST.CustomMonitors.Contract;
using Monitis.API.REST.User;
using Monitis.API.Util;

namespace Monitis.API.REST.CustomMonitors
{
    /// <summary>
    /// Contains API methods for Custom Monitors.
    /// NOTE: now work only with Live API
    /// </summary>
    public class CustomMonitorAPI : MonitisAPIBase
    {
        #region constructors

        public CustomMonitorAPI(string apiKey, APIType apiType)
            : base(apiKey, apiType, APIResources.CustomMonitorsAPIPath)
        {
        }

        #endregion

        #region public methods

        /// <summary>
        /// Get user's all Custom monitors
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public IEnumerable<Monitor> GetMonitorList()
        {
            APIClient apiClient = GetApiClient(ActionNames.GetMonitors);
            Monitor[] response = apiClient.InvokeGet<Monitor[]>();
            return response;
        }

        /// <summary>
        /// Use to add a new Custom monitor.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="authToken"></param>
        /// <param name="descriptor">Configuration of monitor for add</param>
        /// <returns></returns>
        public AddMonitorResponse AddMonitor(String authToken, MonitorDescriptor descriptor)
        {
             if(descriptor == null)
             {
                 throw new ArgumentNullException("descriptor");
             }

            APIClient apiClient = GetApiClient(ActionNames.AddMonitor, authToken);

            String resultParams = Utils.JoinMonitorResultParams(descriptor.ResultParams);

            apiClient.AddParam(ParamNames.ResultParams, resultParams);
            apiClient.AddParam(ParamNames.Name, descriptor.Name);
            apiClient.AddParam(ParamNames.Tag, descriptor.Tag);
            AddMonitorResponse addMonitorResponse = apiClient.InvokePost<AddMonitorResponse>();
            return addMonitorResponse;
        }

        /// <summary>
        /// Add results for the specified Custom monitor.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="authToken"></param>
        /// <param name="monitorID">ID of the monitor to add results for</param>
        /// <param name="data">Data for the monitor</param>
        public void AddResults(String authToken, Int32 monitorID, List<ResultParameter> data)
        {
            Validation.EmptyOrNull(authToken, "authToken");

            //TODO: future optimization, test with big amount of data
            foreach (var resultParameter in data)
            {
                APIClient apiClient = GetApiClient(ActionNames.AddResult, authToken);
               
                apiClient.AddParam(ParamNames.MonitorID, monitorID.ToString());
                apiClient.AddParam(ParamNames.Checktime, Utils.GetMillisecondsCheckTime(resultParameter.Timestamp));
                apiClient.AddParam("results", String.Format("{0}:{1}", resultParameter.Name, resultParameter.Value));

                //TODO: check responce and add to output if some fails
                APIResponce apiResponce = apiClient.InvokePost<APIResponce>();
            }
        }

        #endregion

        
    }
}