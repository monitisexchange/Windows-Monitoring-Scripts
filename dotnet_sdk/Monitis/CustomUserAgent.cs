using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Monitis
{
    public class CustomUserAgent : APIObject
    {

        public CustomUserAgent(String apiKey, String secretKey) :
            base(apiKey, secretKey, Helper.UrlCustomMonitorApi)
        {
        }

        public CustomUserAgent() :
            base(Helper.UrlCustomMonitorApi)
        {
        }

        public enum CustomUserAgentAction
        {
            agentInfo,
            getAgents,
            addAgent,
            editAgent,
            deleteAgent
        }

        public RestResponse GetAgents(String type, bool loadTests, bool loadParams, OutputType output)
        {
            var parameters = new Dictionary<String, Object>();
            if (type != null) parameters.Add("type", type);
            parameters.Add("loadTests", loadTests);
            parameters.Add("loadParams", loadParams);
            parameters.Add("output", output);
            return MakeGetRequest(CustomUserAgentAction.getAgents, parameters);
            ;
        }

        public RestResponse GetAgentInfo(int agentId, OutputType output)
        {
            var parameters = new Dictionary<String, Object>();
            parameters.Add("agentId", agentId);
            parameters.Add("output", output);
            return MakeGetRequest(CustomUserAgentAction.agentInfo, parameters);
        }


        public RestResponse AddAgent(String name, String type, JObject agentParams, int jobPollingInterval,
                                     OutputType output)
        {
            var parameters = new Dictionary<String, Object>();
            parameters.Add("name", StringUtils.UrlEncode(name));
            parameters.Add("type", StringUtils.UrlEncode(type));
            parameters.Add("params", StringUtils.UrlEncode(agentParams.ToString()));
            parameters.Add("jobPollingInterval", jobPollingInterval);
            parameters.Add("output", output);
            return MakePostRequest(CustomUserAgentAction.addAgent, parameters);
        }

        public RestResponse EditAgent(int agentId, String name, JObject agentParams, int jobPollingInterval)
        {
            var parameters = new Dictionary<String, Object>();
            parameters.Add("agentId", agentId);
            parameters.Add("name", StringUtils.UrlEncode(name));
            parameters.Add("params", StringUtils.UrlEncode(agentParams.ToString()));
            parameters.Add("jobPollingInterval", jobPollingInterval);
            return MakePostRequest(CustomUserAgentAction.editAgent, parameters);
        }

        public RestResponse DeleteAgents(int[] agentIds, bool deleteMonitors)
        {
            var parameters = new Dictionary<String, Object>();
            parameters.Add("agentIds", StringUtils.Join(agentIds, ","));
            parameters.Add("deleteMonitors", deleteMonitors);
            return MakePostRequest(CustomUserAgentAction.deleteAgent, parameters);
        }
    }

}