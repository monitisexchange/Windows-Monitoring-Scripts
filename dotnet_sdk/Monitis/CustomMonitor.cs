using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Monitis
{
    public class CustomMonitor : BaseMonitor
    {

        public enum CustomMonitoraction { addAdditionalResults, getAdditionalResults }

        public CustomMonitor(String apiKey, String secretKey)
            : base(apiKey, secretKey, Helper.UrlCustomMonitorApi)
        {

        }

        public CustomMonitor()
            : base(Helper.UrlCustomMonitorApi)
        {

        }

        public RestResponse AddMonitor(int customUserAgentId, String name, String tag, String type, List<MonitorParameter> monitorParams,
                List<MonResultParameter> resultParams, List<MonResultParameter> additionalResultParams)
        {
            var parameters = new Dictionary<String, Object>();
            if (monitorParams != null)
            {
                parameters.Add("monitorParams", URLObject.ToUrlString(monitorParams));
            }
            parameters.Add("resultParams", URLObject.ToUrlString(resultParams));
            if (additionalResultParams != null)
            {
                parameters.Add("additionalResultParams", URLObject.ToUrlString(additionalResultParams));
            }
            parameters.Add("customUserAgentId", customUserAgentId);
            parameters.Add("name", StringUtils.UrlEncode(name));
            parameters.Add("tag", StringUtils.UrlEncode(tag));
            if (type != null) parameters.Add("type", StringUtils.UrlEncode(type));
            return MakePostRequest(MonitorAction.addMonitor, parameters);
        }

        public RestResponse EditMonitor(int monitorId, String name, String tag,
                List<MonitorParameter> monitorParams)
        {
            var parameters = new Dictionary<String, Object>();
            if (monitorParams != null)
            {
                parameters.Add("monitorParams", URLObject.ToUrlString(monitorParams));
            }
            parameters.Add("name", StringUtils.UrlEncode(name));
            parameters.Add("monitorId", monitorId);
            parameters.Add("tag", StringUtils.UrlEncode(tag));
            return MakePostRequest(MonitorAction.editMonitor, parameters);
        }

        public RestResponse AddResult(int monitorId, long checktime, List<MonResult> results)
        {
            var parameters = new Dictionary<String, Object>();
            parameters.Add("results", URLObject.ToUrlString(results));
            parameters.Add("monitorId", monitorId);
            parameters.Add("checktime", checktime);
            return MakePostRequest(MonitorAction.addResult, parameters);
        }

        public RestResponse GetMonitors(String tag, String type, OutputType output)
        {
            var parameters = new Dictionary<String, Object>();
            if (tag != null)
            {
                parameters.Add("tag", StringUtils.UrlEncode(tag));
            }
            if (type != null)
            {
                parameters.Add("type", StringUtils.UrlEncode(type));
            }
            parameters.Add("output", output);
            RestResponse resp = MakeGetRequest(MonitorAction.getMonitors, parameters);
            return resp;
        }

        public RestResponse GetAdditionalResults(int monitorId, long checktime)
        {
            var parameters = new Dictionary<String, Object>();
            parameters.Add("monitorId", monitorId);
            parameters.Add("checktime", checktime);
            return MakeGetRequest(CustomMonitoraction.getAdditionalResults, parameters);
        }

        public void DeleteMonitors(int testId, OutputType? output = null, Validation? validation = null)
        {
            DeleteMonitors(new int[] { testId }, output: output, validation: validation);
        }

        public RestResponse AddAdditionalResults(int monitorId, JArray jsArray, long checktime)
        {
            var parameters = new Dictionary<String, Object>();
            parameters.Add("monitorId", monitorId);
            parameters.Add("results", jsArray.ToString());
            parameters.Add("checktime", checktime);
            return MakePostRequest(CustomMonitoraction.addAdditionalResults, parameters);
        }

        public RestResponse GetMonitorInfo(int monitorId, OutputType output,
                bool excludeHidden)
        {
            var parameters = new Dictionary<String, Object>();
            parameters.Add("monitorId", monitorId);
            parameters.Add("output", output);
            parameters.Add("excludeHidden", excludeHidden);
            return MakeGetRequest(GetAction(MonitorAction.getMonitorInfo), parameters);
        }
        /*
        public static void main(String[] args) throws MonitisException, JSONException {
		
                CustomMonitor customMonitor = new CustomMonitor();
			
                Response response = null;
                //add monitor
                List<MonitorParameter> monitorParams = new ArrayList<MonitorParameter>();
                monitorParams.add(new MonitorParameter("url", "URL", "monitis.com", DataType.STRING, true));
                monitorParams.add(new MonitorParameter("steps", "Steps", "5", DataType.INTEGER, true));
						
                List<MonResultParameter> resultParams = new ArrayList<MonResultParameter>();
			
                resultParams.add(new MonResultParameter("response", "Response", "ms", DataType.INTEGER));
                resultParams.add(new MonResultParameter("status", "Status", "", DataType.STRING));
			
			
                List<MonResultParameter> addResultParams = new ArrayList<MonResultParameter>();
                addResultParams.add(new MonResultParameter("command", "Command", "", DataType.STRING));
                addResultParams.add(new MonResultParameter("duration", "Duration", "ms", DataType.INTEGER));
			
                response = customMonitor.addMonitor(null, "transaction_from_plugin_4", "transaction", "transaction", monitorParams, resultParams, addResultParams);
                System.out.println(response);
                Integer monitorId = new org.json.JSONObject(response.getResponseText()).getInt("data");
			
                //edit monitor
                monitorParams = new ArrayList<MonitorParameter>();
                monitorParams.add(new MonitorParameter("aaa", "a nn aaa", "78", DataType.INTEGER, true));
                response = customMonitor.editMonitor(monitorId, "log parser_with_ad", "logparser", monitorParams);
                System.out.println(response);
			
                //add result
                Long now = TimeUtility.getNowByGMT().getTime().getTime();
                List<MonResult> results = new ArrayList<MonResult>();
                results.add(new MonResult("response", "25"));
                results.add(new MonResult("status", "ok"));
                System.out.println(URLObject.toUrlString(results));
                response = customMonitor.addResult(monitorId, now, results);
                System.out.println(response);
                try {
                    JSONArray addResults = new JSONArray("[{coomand:'open(montiis.com)',duration:10},{command:'click(myId)',duration:2}]");
                    response = customMonitor.addAdditionalResults(monitorId, addResults, now);
                    System.out.println(response);
                } catch (JSONException e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
			
                response = customMonitor.getMonitors(null, "type :; typik", OutputType.XML);
                System.out.println(response);
                response = customMonitor.getMonitorInfo(monitorId, OutputType.XML, false);
                System.out.println(response);
                response = customMonitor.getMonitorResults(monitorId, 2011, 7, 2, 300, OutputType.XML);
                System.out.println(response);
                response = customMonitor.getAdditionalResults(monitorId, now);
                System.out.println(response);
                response = customMonitor.deleteMonitors(new Integer[]{monitorId});
                System.out.println(response);
        }
        */
    }
}