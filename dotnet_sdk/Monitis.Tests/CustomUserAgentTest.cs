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
    class CustomUserAgentTest
    {
        private Authentication authentication = null;
        private CustomUserAgent _agent;

        private int _agentID;

        [TestFixtureSetUp]
        public void Setup()
        {
            CreateCustomUserAgent();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _agent.DeleteAgents(new int[] { _agentID }, true);
        }

        private void CreateCustomUserAgent()
        {
            authentication = new Authentication(apiKey: MonitisAccountInformation.ApiKey,
                                              secretKey: MonitisAccountInformation.SekretKey);
            
            _agent = new CustomUserAgent();
            _agent.SetAuthenticationParams(authentication);

            var a1 = _agent.AddAgent("agent" + DateTime.Now.Ticks.ToString(), "internal", new JObject(), 100000, OutputType.JSON);
            _agentID = JObject.Parse(a1.Content).Value<int>("data");
        }

        [Test]
        public void GetAgentInfo_JSON()
        {
            var s = _agent.GetAgentInfo(_agentID, OutputType.JSON);
            Assert.AreEqual(JObject.Parse(s.Content)["error"], null);
        }

        [Test]
        public void GetAgentInfo_XML()
        {
            var s = _agent.GetAgentInfo(_agentID, OutputType.XML);
            Assert.AreNotEqual(XElement.Parse(s.Content).Name.LocalName, "error");
        }

        [Test]
        public void GetAgents_Xml()
        {
            GetCustomAgents(output: OutputType.XML);
        }

        [Test]
        public void GetAgents_Json()
        {
            GetCustomAgents(output: OutputType.JSON);
        }

        private void GetCustomAgents(OutputType output)
        {
            var monitors = _agent.GetAgents("internal", true, true, output);
            if (output == OutputType.XML)
            {
                XElement.Parse(monitors.Content);
            }
            else if (output == OutputType.JSON)
            {
                JArray.Parse(monitors.Content);
            }
        }
    }
}
