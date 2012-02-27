using System;
using System.Collections.Generic;
using Monitis.API.Common;
using Monitis.API.Domain.Monitors;
using Monitis.API.REST.CustomMonitors.Contract;
using Monitis.API.Util;

namespace Monitis.API.REST.CustomMonitors
{
    /// <summary>
    /// Contains API methods for Custom Monitors.
    /// NOTE: now worok only with Live API
    /// </summary>
    public class CustomMonitorAPI
    {
        /// <summary>
        /// API request path
        /// </summary>
        public const String RequestPath = "/customMonitorApi";

        /// <summary>
        /// Get user's all Custom monitors
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public IEnumerable<Monitor> GetMonitorList(String apiKey)
        {
            Validation.ValidateAPIKey(apiKey);

            APIClient apiClient = new APIClient(APIType.Live, apiKey, RequestPath, ActionNames.GetMonitors);
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
        public AddMonitorResponse AddMonitor(String apiKey, String authToken, MonitorDescriptor descriptor)
        {
            Validation.ValidateAPIKey(apiKey);
            Validation.ValidateAuthToken(authToken);

            APIClient apiClient = new APIClient(APIType.Live, apiKey, RequestPath, ActionNames.AddMonitor, authToken);

            string resultParams = Utils.JoinMonitorResultParams(descriptor.ResultParams);

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
        public void AddResults(String apiKey, String authToken, Int32 monitorID, List<ResultParameter> data)
        {
            Validation.ValidateAPIKey(apiKey);
            Validation.ValidateAuthToken(authToken);

            foreach (var resultParameter in data)
            {
                APIClient apiClient = new APIClient(APIType.Live, apiKey, RequestPath, ActionNames.AddResult, authToken);
                apiClient.AddParam(ParamNames.MonitorID, monitorID.ToString());
                apiClient.AddParam(ParamNames.Checktime, Utils.GetMillisecondsCheckTime(resultParameter.Timestamp));
                apiClient.AddParam("results", String.Format("{0}:{1}", resultParameter.Name, resultParameter.Value));
                APIResponce apiResponce = apiClient.InvokePost<APIResponce>();
            }

        }

       
    }
}