using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Monitis.Structures;
using RestSharp;

namespace Monitis
{
    public class ExternalMonitor : BaseMonitor
    {
        #region Enums for parameters

        #region CheckInterval enum

        /// <summary>
        /// check interval(min)
        /// </summary>
        public enum CheckInterval
        {
            [XmlEnum("1")] one = 1,
            [XmlEnum("3")] three = 3,
            [XmlEnum("5")] five = 5,
            [XmlEnum("10")] ten = 10,
            [XmlEnum("15")] fifteen = 15,
            [XmlEnum("20")] twenty = 20,
            [XmlEnum("30")] thirty = 30,
            [XmlEnum("40")] forty = 40,
            [XmlEnum("60")] sixty = 60
        }

        #endregion

        #region DetailedTestType enum

        /// <summary>
        /// specifies the request method. Used for HTTP and HTTPS. 
        /// </summary>
        public enum DetailedTestType
        {
            [XmlEnum("null")] NOTSET = 0,
            GET = 1,
            POST = 2,
            PUT = 3,
            DELETE = 4
        }

        #endregion

        #region Status enum

        /// <summary>
        ///  test status
        /// </summary>
        public enum Status
        {
            [XmlEnum("0")] OK = 0,
            [XmlEnum("1")] warning = 1,
            [XmlEnum("2")] NOK = 2,
            [XmlEnum("null")] NA = 3
        }

        #endregion

        #region TestType enum

        /// <summary>
        /// Supported test types
        /// </summary>
        public enum TestType
        {
            http,
            https,
            ftp,
            ping,
            ssh,
            dns,
            mysql,
            udp,
            tcp,
            sip,
            smtp,
            imap,
            pop
        }

        #endregion

        private enum ExternalMonitorAction
        {
            addExternalMonitor,
            editExternalMonitor,
            deleteExternalMonitor,
            tests,
            tags,
            tagtests,
            testinfo,
            testresult,
            topexternal,
            testsLastValues,
            locations,
            suspendExternalMonitor,
            activateExternalMonitor
        }

        #endregion

        public override Enum GetAction(MonitorAction action)
        {
            Enum eEnum = null;
            switch (action)
            {
                case MonitorAction.getMonitors:
                    eEnum = ExternalMonitorAction.tests;
                    break;
                case MonitorAction.getMonitorInfo:
                    eEnum = ExternalMonitorAction.testinfo;
                    break;
                case MonitorAction.getMonitorResults:
                    eEnum = ExternalMonitorAction.testresult;
                    break;
                case MonitorAction.suspendMonitors:
                    eEnum = ExternalMonitorAction.suspendExternalMonitor;
                    break;
                case MonitorAction.activateMonitors:
                    eEnum = ExternalMonitorAction.activateExternalMonitor;
                    break;
            }
            return eEnum;
        }

        /// <summary>
        /// This action is used to add a new External monitor. 
        /// </summary>
        /// <param name="type">type of the test</param>
        /// <param name="name">the name of the test</param>
        /// <param name="url">url of the test</param>
        /// <param name="interval">check interval(min)</param>
        /// <param name="tag">tag of the test</param>
        /// <param name="detailedTestType">specifies the request method. Used for HTTP and HTTPS. Default value is GET. </param>
        /// <param name="timeout">test timeout in ms. Default value is 10000</param>
        /// <param name="overSSL">if true, requests will be sent via SSL. Can be set for FTP, UDP, TCP, SMTP, IMAP, POP test types.</param>
        /// <param name="postData">data to send during POST request, e.g m_U=asd&m_P=asd</param>
        /// <param name="contentMatchFlag">set to true if there is string to match in response text otherwise false.</param>
        /// <param name="contentMatchString">text to match in the response</param>
        /// <param name="testParams">additional test parameters 
        /// TODO: http://monitis.com/api/api.html#addExternalMonitor - create type for MySQL and DNS?</param>
        /// <param name="uptimeSLA">min allowed uptime(%)</param>
        /// <param name="responseSLA">max allowed response time in seconds</param>
        /// <param name="basicAuthUser">userName for authentication</param>
        /// <param name="basicAuthPass">password for authentication</param>
        /// <param name="output">Output type - JSON or XML</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        /// <param name="locationIds">comma separated ids of the locations to add test for</param>
        /// <returns>Add ExternalMonitor Response</returns>
        public AddExternalMonitorResponse AddMonitor(
            TestType type,
            string name,
            string url,
            CheckInterval interval,
            string tag,
            int[] locationIds,
            DetailedTestType detailedTestType = DetailedTestType.GET,
            int timeout = 10000,
            bool? overSSL = null,
            string postData = null,
            bool? contentMatchFlag = null,
            string contentMatchString = null,
            Dictionary<string, string> testParams = null,
            int? uptimeSLA = null,
            int? responseSLA = null,
            string basicAuthUser = null,
            string basicAuthPass = null,
            OutputType? output = null,
            Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.name, name);
            parameters.Add(Params.tag, tag);
            parameters.Add(Params.locationIds, String.Join(",", locationIds));
            parameters.Add(Params.url, url);
            parameters.Add(Params.type, type);
            parameters.Add(Params.interval, (int) interval); /*minutes - int value*/
            AddIfNotNull(parameters, Params.contentMatchString, contentMatchString);
            AddIfNotNull(parameters, Params.contentMatchFlag, Helper.BoolToInt(contentMatchFlag));
            AddIfNotNull(parameters, Params.basicAuthUser, basicAuthUser);
            AddIfNotNull(parameters, Params.basicAuthPass, basicAuthPass);
            AddIfNotNull(parameters, Params.postData, postData);
            if (null != testParams && testParams.Count > 0)
            {
                string testParamsString = Helper.MapToURLString(testParams);
                AddIfNotNull(parameters, Params.@params, testParamsString);
            }
            parameters.Add(Params.detailedTestType, (int) detailedTestType);
            AddIfNotNull(parameters, Params.timeout, timeout);
            AddIfNotNull(parameters, Params.uptimeSLA, uptimeSLA);
            AddIfNotNull(parameters, Params.responseSLA, responseSLA);
            AddIfNotNull(parameters, Params.overSSL, Helper.BoolToInt(overSSL));

            RestResponse response = MakePostRequest(ExternalMonitorAction.addExternalMonitor, parameters,
                                                    output: outputType, validation: validation);
            Helper.CheckStatus(response, output: outputType);
            var addExternalMonitorResponse = new AddExternalMonitorResponse();

            //Fill information from response to struct
            Int32.TryParse(Helper.GetValueOfKey(response, Params.isTestNew, outputType),
                           out addExternalMonitorResponse.isTestNew);
            Int32.TryParse(Helper.GetValueOfKey(response, Params.testId, outputType),
                           out addExternalMonitorResponse.testId);
            DateTime.TryParse(Helper.GetValueOfKey(response, Params.startDate, outputType),
                              out addExternalMonitorResponse.startDate);
            return addExternalMonitorResponse;
        }


        /// <summary>
        /// This action is used for External monitor editing. 
        /// </summary>
        /// <param name="testId">id of the test to edit</param>
        /// <param name="name">the name of the test</param>
        /// <param name="url">url of the test</param>
        /// <param name="locationIdIntervals">location id and check interval pairs</param>
        /// <param name="tag">tag of the test</param>
        /// <param name="timeout">test timeout in ms</param>
        /// <param name="contentMatchString">text to match in the response</param>
        /// <param name="maxValue">max response time in ms</param>
        /// <param name="uptimeSLA">min allowed uptime(%)</param>
        /// <param name="responseSLA">max allowed response time in seconds</param>
        /// <param name="output">Output type - JSON or XML</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void EditMonitor(
            int testId,
            string name,
            string url,
            Dictionary<int, CheckInterval> locationIdIntervals,
            string tag,
            int timeout = 10000,
            string contentMatchString = null,
            int? maxValue = null,
            int? uptimeSLA = null,
            int? responseSLA = null,
            OutputType? output = null,
            Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.testId, testId);
            parameters.Add(Params.name, name);
            parameters.Add(Params.tag, tag);
            if (null != locationIdIntervals)
            {
                List<string> locationIdIntrvalsList = locationIdIntervals.Select(keyValuePair =>
                                                                                 keyValuePair.Key + "-" +
                                                                                 (int) keyValuePair.Value).ToList();
                parameters.Add(Params.locationIds, String.Join(",", locationIdIntrvalsList));
            }
            parameters.Add(Params.url, url);
            AddIfNotNull(parameters, Params.contentMatchString, contentMatchString);
            AddIfNotNull(parameters, Params.timeout, timeout);
            AddIfNotNull(parameters, Params.maxValue, maxValue);
            AddIfNotNull(parameters, Params.uptimeSLA, uptimeSLA);
            AddIfNotNull(parameters, Params.responseSLA, responseSLA);
            RestResponse response = MakePostRequest(ExternalMonitorAction.editExternalMonitor, parameters,
                                                    output: outputType, validation: validation);
            Helper.CheckStatus(response, outputType);
        }

        /// <summary>
        /// This action is used to get user's all locations for External monitors. 
        /// </summary>
        /// <param name="output">Output type - JSON or XML</param>
        /// <returns>locations for external monitors</returns>
        public Location[] GetLocations(OutputType? output = null)
        {
            Location[] locations = null;
            OutputType outputType = GetOutput(output);
            RestResponse restResponse = MakeGetRequest(ExternalMonitorAction.locations, output: output);
            locations = Helper.DeserializeObject<Location[]>(restResponse, outputType, Params.locations);
            return locations;
        }

        /// <summary>
        /// This action is used to get user's all External monitors. 
        /// </summary>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <returns>external monitors</returns>
        public Test[] GetMonitors(OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            RestResponse response = MakeGetRequest(GetAction(MonitorAction.getMonitors), output: outputType);
            Test[] tests = ParseExternalMonitors(response, outputType);
            return tests;
        }

        /// <summary>
        /// This action is used to get information regarding the specified External monitor. 
        /// </summary>
        /// <param name="monitorId">id of the test to get information for</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <returns>external monitor information</returns>
        public new ExternalMonitorInformation GetMonitorInfo(int monitorId, OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.testId, monitorId);
            RestResponse response = MakeGetRequest(GetAction(MonitorAction.getMonitorInfo), parameters,
                                                   output: outputType);
            //TODO: xml - incorrect deserialization. Temp fix - ignore @params for xml
            var monitorsInfo = Helper
                .DeserializeObject<ExternalMonitorInformation>(response, outputType, Params.result);
            //TODO: fix @params - incrorrect value
            Dictionary<string, string> temp = monitorsInfo.@params;
            int t = temp.Count;
            t++;
            return monitorsInfo;
        }

        /// <summary>
        /// This action is used to get last results of user's all External monitors. 
        /// </summary>
        /// <param name="output">Output type - JSON or XML</param>
        /// <param name="locationIds"></param>
        /// <returns>snapshot result - locations with monitors results</returns>
        public LocationWithMonitorsResults[] GetSnapshot(OutputType? output = null, params int[] locationIds)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            if (locationIds != null && locationIds.Length > 0)
            {
                parameters.Add(Params.locationIds, String.Join(",", locationIds));
            }
            RestResponse restResponse = MakeGetRequest(ExternalMonitorAction.testsLastValues, parameters,
                                                       output: outputType);
            var result =
                Helper.DeserializeObject<LocationWithMonitorsResults[]>(restResponse,
                                                                        outputType,
                                                                        Params.result);
            return result;
        }

        /// <summary>
        /// This action is used to get all tags for user's External monitors. 
        /// </summary>
        /// <param name="output">Output type - JSON or XML</param>
        /// <returns>tags</returns>
        public Tag[] GetTags(OutputType? output = null)
        {
            Tag[] tags = null;
            OutputType outputType = GetOutput(output);
            RestResponse response = MakeGetRequest(ExternalMonitorAction.tags, output: output);
            if (outputType == OutputType.JSON)
            {
                var tagsJson = Helper.DeserializeObject
                    <TagsExternalMonitorJson>(
                        response, outputType, Params.result);
                tags = tagsJson.tags;
            }
            else if (outputType == OutputType.XML)
            {
                tags = Helper.DeserializeObject<Tag[]>(response, outputType, Params.result);
            }

            return tags;
        }

        /// <summary>
        /// This action is used to get External monitors for the specified tag. 
        /// </summary>
        /// <param name="tag">tag to get monitors for</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <returns>external monitors for the specified tag</returns>
        public Test[] GetTagTests(string tag, OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.tag, tag);
            RestResponse response = MakeGetRequest(ExternalMonitorAction.tagtests, parameters, output: outputType);
            Test[] tests = ParseExternalMonitors(response, outputType);
            return tests;
        }

        /// <summary>
        /// Parse response to array of Test
        /// </summary>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="response">response with tests to parse</param>
        /// <returns>Tests array</returns>
        private Test[] ParseExternalMonitors(RestResponse response, OutputType output)
        {
            Test[] tests = null;
            if (output == OutputType.XML)
            {
                tests = Helper.DeserializeObject<Test[]>(response, output, Params.result);
            }
            else if (output == OutputType.JSON)
            {
                var testJson = Helper.DeserializeObject<TestJson>(response, output,
                                                                  Params.result);
                tests = testJson.testList;
            }
            return tests;
        }

        /// <summary>
        /// This action is used to get tests' last results. 
        /// </summary>
        /// <param name="tag">tag to get top results for</param>
        /// <param name="limit">max number of top results to get</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <returns>ExternalMonitorTop result</returns>
        public ExternalMonitorTop GetTops(string tag = null, int? limit = null, OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            RestResponse response = GetTops(ExternalMonitorAction.topexternal, tag, limit, false, outputType);
            var externalMonitorTop = Helper.DeserializeObject<ExternalMonitorTop>(response, outputType,
                                                                                  Params.data);
            return externalMonitorTop;
        }

        #region DeleteMonitors

        /// <summary>
        /// This action is used for External monitor deleting. 
        /// </summary>
        /// <param name="output">Output type - JSON or XML</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        /// <param name="testIds">ids of the tests to delete</param>
        public override void DeleteMonitors(int[] testIds, OutputType? output = null, Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.testIds, String.Join(",", testIds));
            RestResponse response = MakePostRequest(ExternalMonitorAction.deleteExternalMonitor, parameters,
                                                    output: outputType, validation: validation);
            Helper.CheckStatus(response, outputType);
        }

        /// <summary>
        /// This action is used for External monitors deleting. 
        /// </summary>
        /// <param name="output">Output type - JSON or XML</param>
        /// <param name="validation">Output type - JSON or XML</param>
        /// <param name="testId">id of the test to delete</param>
        public void DeleteMonitors(int testId, OutputType? output = null, Validation? validation = null)
        {
            DeleteMonitors(new[] {testId}, output: output, validation: validation);
        }

        #endregion

        #region SuspendMonitors

        /// <summary>
        /// This action is used for suspending External monitors. 
        /// </summary>
        /// <param name="monitorIds"> ids of the monitors to suspend</param>
        /// <param name="output">Output type - JSON or XML</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void SuspendMonitors(int[] monitorIds, OutputType? output = null, Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            RestResponse response = base.SuspendMonitors(monitorIds: monitorIds, tag: null, output: output,
                                                         validation: validation);
            Helper.CheckStatus(response, outputType);
        }

        /// <summary>
        /// This action is used for suspending External monitors. 
        /// </summary>
        /// <param name="monitorId">id of the monitors to suspend</param>
        /// <param name="output">Output type - JSON or XML</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void SuspendMonitors(int monitorId, OutputType? output = null, Validation? validation = null)
        {
            SuspendMonitors(new[] {monitorId});
        }

        /// <summary>
        /// This action is used for suspending External monitors. 
        /// </summary>
        /// <param name="tag">tests with this tag will be suspended.</param>
        /// <param name="output">Output type - JSON or XML</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void SuspendMonitors(string tag, OutputType? output = null, Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            RestResponse response = base.SuspendMonitors(monitorIds: new int[] {}, tag: tag, output: output,
                                                         validation: validation);
            Helper.CheckStatus(response, outputType);
        }

        #endregion

        #region ActivateMonitor

        /// <summary>
        /// This action is used for suspending External monitors. 
        /// </summary>
        /// <param name="monitorIds"> ids of the monitors to activate</param>
        /// <param name="output">Output type - JSON or XML</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void ActivateMonitors(int[] monitorIds, OutputType? output = null, Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            RestResponse response = base.ActivateMonitors(
                monitorIds: monitorIds,
                tag: null,
                output: output,
                validation: validation);
            Helper.CheckStatus(response, outputType);
        }

        /// <summary>
        /// This action is used for External monitor activating. 
        /// </summary>
        /// <param name="monitorId">id of the monitors to activate</param>
        /// <param name="output">Output type - JSON or XML</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void ActivateMonitors(int monitorId, OutputType? output = null, Validation? validation = null)
        {
            ActivateMonitors(new[] {monitorId});
        }

        /// <summary>
        /// This action is used for External monitor activating. 
        /// </summary>
        /// <param name="tag">tests with this tag will be activated.</param>
        /// <param name="output">Output type - JSON or XML</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void ActivateMonitors(string tag, OutputType? output = null, Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            RestResponse response = base.ActivateMonitors(
                monitorIds: new int[] {},
                tag: tag,
                output: output,
                validation: validation);
            Helper.CheckStatus(response, outputType);
        }

        #endregion

        #region Constructors

        public ExternalMonitor()
        {
        }

        public ExternalMonitor(string apiKey, string secretKey)
            : base(apiKey, secretKey)
        {
        }

        public ExternalMonitor(string apiKey, string secretKey, string apiUrl)
            : base(apiKey, secretKey, apiUrl)
        {
        }

        public ExternalMonitor(Authentication authentication)
            : base(authentication)
        {
        }

        #endregion
    }
}