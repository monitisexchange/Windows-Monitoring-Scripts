using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace Monitis.Tests
{
    [TestFixture]
    public class CustomMonitorTests
    {
        private const string tagNew = "tag new";
        private Authentication authentication = null;
        private CustomMonitor customMonitor = null;
        private CustomUserAgent agent = null;

        private List<int> monitorsToDelete = new List<int>();
        private string TestCustomMonitorName = "testCustomMonitor";
        private int _agentID;
        private int _customMonitorID;

        [TestFixtureSetUp]
        public void Setup()
        {

            CreateCustomMonitor();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            monitorsToDelete.Add(_customMonitorID);
            customMonitor.DeleteMonitors(monitorsToDelete.ToArray());
            monitorsToDelete.Clear();
            agent.DeleteAgents(new int[] { _agentID }, true);
        }

        #region GetMonitors

        public void CreateCustomMonitor()
        {
            authentication = new Authentication(apiKey: MonitisAccountInformation.ApiKey,
                                              secretKey: MonitisAccountInformation.SekretKey);
            customMonitor = new CustomMonitor();
            customMonitor.SetAuthenticationParams(authentication);
            agent = new CustomUserAgent();
            agent.SetAuthenticationParams(authentication);

            var a1 = agent.AddAgent("TestAgent1" + DateTime.Now.Ticks.ToString(), "internal", new JObject(), 100000, OutputType.JSON);
            _agentID = JObject.Parse(a1.Content).Value<int>("data");

            customMonitor = new CustomMonitor();
            customMonitor.SetAuthenticationParams(authentication);

            MonitorParameter param = new MonitorParameter("param1", "param1d", "val", DataType.String, false);
            MonResultParameter resParam = new MonResultParameter("MonResparam1", "MonResparam1d", "MonResval",
                                                                 DataType.String);
            MonResultParameter resAddParam = new MonResultParameter("MonAddResparam1", "MonAddResparam1d",
                                                                    "MonAddResval", DataType.String);

            var s = customMonitor.AddMonitor(_agentID, TestCustomMonitorName + DateTime.Now.Ticks.ToString(), "Test", "internal",
                                             new List<MonitorParameter>() { param },
                                             new List<MonResultParameter>() { resParam },
                                             new List<MonResultParameter>() { resAddParam });
            _customMonitorID = JObject.Parse(s.Content).Value<int>("data");


            GetTestMonitor();
        }

        [Test]
        public void GetTop_XML()
        {
            customMonitor.GetTops(CustomMonitor.CustomMonitoraction.addAdditionalResults, "internal", 100, true,
                                          OutputType.XML);
        }

        [Test]
        public void GetTop_JSON()
        {
            customMonitor.GetTops(CustomMonitor.CustomMonitoraction.addAdditionalResults, "internal", 100, true,
                                          OutputType.JSON);
        }

        [Test]
        public void AddAdditionalResultToCusomMonitor()
        {
            JObject param1 = new JObject(new JProperty("MonResparam1", "paramVal"));
            var s = customMonitor.AddAdditionalResults(_customMonitorID, new JArray(param1), Helper.GetCurrentTime());
            Assert.AreEqual(JObject.Parse(s.Content)["status"].Value<string>(), "ok");
        }

        [Test]
        public void GetMonitorInfo_JSON()
        {
            var s = customMonitor.GetMonitorInfo(_customMonitorID, OutputType.JSON, false);
            Assert.AreEqual(JObject.Parse(s.Content)["error"], null);
        }

        [Test]
        public void GetMonitorInfo_XML()
        {
            var s = customMonitor.GetMonitorInfo(_customMonitorID, OutputType.XML, false);
            Assert.AreNotEqual(XElement.Parse(s.Content).Name.LocalName, "error");
        }

        [Test]
        public void GetAdditionalResults()
        {
            var s = customMonitor.GetAdditionalResults(_customMonitorID, Helper.GetCurrentTime());
        }

        [Test]
        public void AddResultToCusomMonitor()
        {
            MonResult param = new MonResult("MonResparam1", "paramVal222");
            var s = customMonitor.AddResult(_customMonitorID, Helper.GetCurrentTime(), new List<MonResult>() { param });
            Assert.AreEqual(JObject.Parse(s.Content)["status"].Value<string>(), "ok");
        }

        [Test]
        public void GetCustomMonitors_Xml()
        {
            GetCustomMonitors(output: OutputType.XML);
        }

        [Test]
        public void GetCustomMonitors_Json()
        {
            GetCustomMonitors(output: OutputType.JSON);
        }

        public void GetCustomMonitors(OutputType output)
        {
            var monitors = customMonitor.GetMonitors(output);
            if (output == OutputType.XML)
            {
                XElement.Parse(monitors.Content);
            }
            else if (output == OutputType.JSON)
            {
                JArray.Parse(monitors.Content);
            }
        }

        private int GetTestMonitor()
        {
            var monitors = customMonitor.GetMonitors(OutputType.JSON);
            foreach (var obj in JArray.Parse(monitors.Content))
            {
                if (obj["name"].Value<string>() == TestCustomMonitorName)
                    return obj["id"].Value<int>();
            }
            throw new Exception("Can't get monitors");
        }

        #endregion
    }
}
