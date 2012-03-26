using System;
using System.Collections.Generic;
using RestSharp;

namespace Monitis
{
    public class BaseMonitor : APIObject
    {
        public enum MonitorAction
        {
            addMonitor,
            editMonitor,
            deleteMonitor,
            addResult,
            getMonitors,
            getMonitorInfo,
            getMonitorResults,
            tops,
            suspendMonitors,
            activateMonitors
        }


        #region Constructors

        protected BaseMonitor()
        {
        }

        protected BaseMonitor(string apiKey, string secretKey)
            : base(apiKey, secretKey)
        {

        }

        protected BaseMonitor(string apiKey, string secretKey, string apiUrl)
            : base(apiKey, secretKey, apiUrl)
        {

        }

        protected BaseMonitor(Authentication authentication)
            : base(authentication)
        {

        }

        #endregion

        protected virtual RestResponse GetMonitorInfo(int monitorId, OutputType? output = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.monitorId, monitorId);
            RestResponse resp = MakeGetRequest(GetAction(MonitorAction.getMonitorInfo), parameters, output: output);
            return resp;
        }


        protected virtual RestResponse GetMonitors(OutputType? output = null)
        {
            RestResponse resp = MakeGetRequest(GetAction(MonitorAction.getMonitors), output: output);
            return resp;
        }

        protected virtual RestResponse GetMonitorResults(int monitorId, int year, int month, int day, int? timezone = null,
                                              OutputType? output = null, params int[] locationIds)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.monitorId, monitorId);
            parameters.Add(Params.year, year);
            parameters.Add(Params.month, month);
            parameters.Add(Params.day, day);
            AddIfNotNull(parameters, Params.timezone, timezone);
            
            if (locationIds != null&& locationIds.Length>0)
            {
                parameters.Add(Params.locationIds, String.Join(",", locationIds));
            }
            RestResponse resp = MakeGetRequest(GetAction(MonitorAction.getMonitorResults), parameters, output);
            return resp;
        }

        protected virtual void DeleteMonitors(int[] monitorIds,OutputType? output = null, Validation? validation = null)
        {
            var parameters = new Dictionary<string, object>();
            if (monitorIds != null)
            {
                parameters.Add(Params.monitorId, string.Join(",", monitorIds));
            }
            RestResponse resp = MakePostRequest(GetAction(MonitorAction.deleteMonitor), parameters,
                                                output: output, validation: validation);
        }

        protected virtual RestResponse SuspendMonitors(int[] monitorIds, string tag, OutputType? output = null, Validation? validation = null)
        {
            var parameters = new Dictionary<string, object>();
            if (monitorIds != null && monitorIds.Length > 0)
            {
                parameters.Add(Params.monitorIds, String.Join(",", monitorIds));
            }
            AddIfNotNull(parameters, Params.tag, tag);
            RestResponse resp = MakePostRequest(GetAction(MonitorAction.suspendMonitors), parameters,output:output,validation:validation);
            return resp;
        }

        protected virtual RestResponse ActivateMonitors(int[] monitorIds, string tag, OutputType? output = null, Validation? validation = null)
        {
            var parameters = new Dictionary<string, object>();
            AddIfNotNull(parameters, Params.monitorIds, string.Join(",", monitorIds));
            AddIfNotNull(parameters, Params.tag, tag);
            RestResponse resp = MakePostRequest(GetAction(MonitorAction.activateMonitors), parameters, output: output, validation: validation);
            return resp;
        }

        protected virtual RestResponse GetTops(Enum action, string tag, int? limit, bool detailedResults, OutputType? output=null)
        {
            var parameters = new Dictionary<string, object>();
            AddIfNotNull(parameters, Params.limit, limit);
            AddIfNotNull(parameters,Params.tag, tag);
            parameters.Add(Params.detailedResults, detailedResults);
            RestResponse resp = MakeGetRequest(action, parameters, output: output);
            return resp;
        }

        protected virtual Enum GetAction(MonitorAction action)
        {
            return action;
        }
    }
}