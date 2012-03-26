using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monitis.Structures;
using NUnit.Framework;

namespace Monitis.Tests
{
    [TestFixture]
    public class HelperTests
    {
        #region DeserializeObject

        [Test]
        public void DeserializeObject_ExternalMonitor_GetMonitors_Xml()
        {
            string testXmlString =
                @"<result>
	                <test id='80856' isSuspended='0' type='http'><![CDATA[127.0.0.2_http]]></test>
	                <test id='80857' isSuspended='0' type='http'><![CDATA[213.178.59.17_http]]></test>	
	                <test id='80858' isSuspended='0' type='ping'><![CDATA[213.178.59.17_ping]]></test>
                </result>";

            Structures.Test[] tests = Monitis.Helper.DeserializeObject<Structures.Test[]>(testXmlString, OutputType.XML,
                                                                                          Params.result);
            if (tests.Length == 3 && tests[0].name == "127.0.0.2_http")
            {
                Assert.Pass("Correct deserialize to  Structures.Test[] ");
            }
            else
            {
                Assert.Fail("Incorrect deserialization");
            }
        }

        [Test]
        public void DeserializeObject_ExternalMonitor_GetMonitors_Json()
        {
            string testJsonString =
                @"{""testList"":[{""id"":80856,""isSuspended"":0,""name"":""127.0.0.2_http"",""type"":""http""},{""id"":80857,""isSuspended"":0,""name"":""213.178.59.17_http"",""type"":""http""},{""id"":80858,""isSuspended"":0,""name"":""213.178.59.17_ping"",""type"":""ping""}]}";
            Structures.TestJson test = Monitis.Helper.DeserializeObject<Structures.TestJson>(testJsonString,
                                                                                             OutputType.JSON,
                                                                                             Params.result);
            Structures.Test[] tests = test.testList;
        }


        [Test]
        public void DeserializeObject_SubAccountPage_GetSubAccountPages_Xml()
        {
            string testXmlString =
                @"<subaccounts>\r\n\t\t<id>17712</id>\r\n\t<account>a7dm64goi607o0w@mailforspam.com</account>\r\n\t<pages>\r\n\t\t<page>Summary</page>\r\n\t</pages>\r\n\t<id>17728</id>\r\n\t<account>a3765myd6phdg0x@mailforspam.com</account>\r\n\t<pages>\r\n\t</pages>\r\n</subaccounts>";
            //TODO:fix after fixing response from server (add 'subaccount' tag for each item)
            testXmlString = testXmlString.Replace("<id>", "<" + Params.subaccountpage + "><id>");
            testXmlString = testXmlString.Replace("</pages>", "</pages>" + "</" + Params.subaccountpage + ">");

            Structures.SubAccountPage[] ob =
                Monitis.Helper.DeserializeObject<Monitis.Structures.SubAccountPage[]>(testXmlString, OutputType.XML,
                                                                                      "subaccounts");
        }

        [Test]
        public void DeserializeObject_SubAccountPage_GetSubAccountPages_Json()
        {
            string testJsonString =
                "[{\"id\":17712,\"pages\":[\"Summary\"],\"account\":\"a7dm64goi607o0w@mailforspam.com\"},{\"id\":17728,\"pages\":[],\"account\":\"a3765myd6phdg0x@mailforspam.com\"}]";
            Structures.SubAccountPage[] ob =
                Monitis.Helper.DeserializeObject<Monitis.Structures.SubAccountPage[]>(testJsonString, OutputType.JSON,
                                                                                      "subaccounts");
        }


        [Test]
        public void DeserializeObject_ExternalMonitor_GetMonitorInfo_Json()
        {
            OutputType output = OutputType.JSON;
            string testJsonString =
                "{\"startDate\":\"2012-03-20 16:23\",\"postData\":null,\"interval\":5,\"testId\":82252,\"detailedType\":null,\"authPassword\":null,\"tag\":\"tag new\",\"authUsername\":null,\"params\":{},\"type\":\"ping\",\"url\":\"www.yandex.ru\",\"locations\":[{\"id\":4,\"name\":\"UK\",\"checkInterval\":5,\"fullName\":\"UK\"},{\"id\":11,\"name\":\"NL\",\"checkInterval\":5,\"fullName\":\"Netherlands\"}],\"name\":\"test pinga4bm3\",\"sla\":{},\"match\":null,\"matchText\":null,\"timeout\":10000}";
            Structures.ExternalMonitorInformation ob =
                Monitis.Helper.DeserializeObject
                    <Monitis.Structures.ExternalMonitorInformation>(testJsonString, output, "result");
        }

        [Test]
        public void DeserializeObject_ExternalMonitor_GetMonitorInfo_Xml()
        {
            OutputType output = OutputType.XML;

            //string testXmlString =
            //    @"<result><startDate>2012-03-20 18:44</startDate><postData>null</postData><interval>5</interval><testId>82331</testId><detailedType>null</detailedType><authPassword>null</authPassword><tag>tag new</tag><authUsername>null</authUsername><params><3>aa</3><dddd>eeee</dddd><bbb>cccc</bbb></params><type>dns</type><url>www.yandex.ru</url><locations><location><id>4</id><name>UK</name><checkInterval>5</checkInterval><fullName>UK</fullName></location><location><id>11</id><name>NL</name><checkInterval>5</checkInterval><fullName>Netherlands</fullName></location></locations><name>test pingawrt8</name><sla></sla><matchText>null</matchText><match>null</match><timeout>10000</timeout></result>";
            //Structures.ExternalMonitorInformation ob =
            //    Monitis.Helper.DeserializeObject<Monitis.Structures.ExternalMonitorInformation>
            //        (testXmlString, output, "result");

            string testXmlString =
                @"<result><startDate>2012-03-20 17:21</startDate><postData>null</postData><interval>5</interval><testId>82276</testId><detailedType>null</detailedType><authPassword>null</authPassword><tag>tag new</tag><authUsername>null</authUsername><params></params><type>ping</type><url>www.yandex.ru</url><locations><location><id>4</id><name>UK</name><checkInterval>5</checkInterval><fullName>UK</fullName></location><location><id>11</id><name>NL</name><checkInterval>5</checkInterval><fullName>Netherlands</fullName></location></locations><name>test pingah25h</name><sla></sla><matchText>null</matchText><match>null</match><timeout>10000</timeout></result>";
            var ob =
                Monitis.Helper.DeserializeObject<Monitis.Structures.ExternalMonitorInformation>
                    (testXmlString, output, "result");
        }

        [Test]
        public void DeserializeObject_ExternalMonitor_GetSnapshot_Xml()
        {
            OutputType output = OutputType.XML;
            string testString =
                /*xml response ExternalMonitor GetSnapshot method - 3 locations, 1 test per location*/
                "<result>\r\n\t<location id='1' name='US-MID'>\r\n\t\t<test>\r\n\t\t\t<name><![CDATA[google.com_ping]]></name>\r\n\t\t\t<performance>33.033</performance>\r\n\t\t\t<status>0</status>\r\n\t\t\t<time>Mar 21, 2012 11:20:14 AM</time>\r\n\t\t</test>\r\n\t</location>\r\n\t<location id='10' name='US-WST'>\r\n\t\t<test>\r\n\t\t\t<name><![CDATA[google.com_ping]]></name>\r\n\t\t\t<performance>3.412</performance>\r\n\t\t\t<status>0</status>\r\n\t\t\t<time>Mar 21, 2012 11:20:46 AM</time>\r\n\t\t</test>\r\n\t</location>\r\n\t<location id='9' name='US-EST'>\r\n\t\t<test>\r\n\t\t\t<name><![CDATA[google.com_ping]]></name>\r\n\t\t\t<performance>2.906</performance>\r\n\t\t\t<status>0</status>\r\n\t\t\t<time>Mar 21, 2012 11:17:14 AM</time>\r\n\t\t</test>\r\n\t</location>\r\n</result>";
            Structures.LocationWithMonitorsResults[] snapshot =
                Helper.DeserializeObject<Structures.LocationWithMonitorsResults[]>(testString,
                                                                                   output,
                                                                                   Params.result);
            if (snapshot.Length == 3 && snapshot[0].data.Length == 1)
            {
                Assert.Pass("Parse is correct");
            }
            else
            {
                Assert.Fail("Incorrect parse!");
            }
        }

        [Test]
        public void DeserializeObject_ExternalMonitor_GetSnapshot_Json()
        {
            OutputType output = OutputType.JSON;
            string testString =
                "[{\"id\":4,\"name\":\"UK\",\"data\":[{\"id\":82348,\"testType\":\"dns\",\"time\":\"20 Mar 2012 20:18:05 GMT\",\"perf\":0.0,\"status\":\"NOK\",\"tag\":\"tag new\",\"name\":\"test pinga5ygi\",\"frequency\":null,\"timeout\":10000}],\"locationShortName\":\"UK\"},{\"id\":11,\"name\":\"Netherlands\",\"data\":[{\"id\":82348,\"testType\":\"dns\",\"time\":\"20 Mar 2012 20:18:03 GMT\",\"perf\":0.0,\"status\":\"NOK\",\"tag\":\"tag new\",\"name\":\"test pinga5ygi\",\"frequency\":null,\"timeout\":10000}],\"locationShortName\":\"NL\"},{\"id\":4,\"name\":\"UK\",\"data\":[{\"id\":82365,\"testType\":\"dns\",\"time\":\"20 Mar 2012 20:19:05 GMT\",\"perf\":0.0,\"status\":\"NOK\",\"tag\":\"tag new\",\"name\":\"test pinga1qkf\",\"frequency\":null,\"timeout\":10000}],\"locationShortName\":\"UK\"},{\"id\":11,\"name\":\"Netherlands\",\"data\":[{\"id\":82365,\"testType\":\"dns\",\"time\":\"20 Mar 2012 20:04:05 GMT\",\"perf\":0.0,\"status\":\"NOK\",\"tag\":\"tag new\",\"name\":\"test pinga1qkf\",\"frequency\":null,\"timeout\":10000}],\"locationShortName\":\"NL\"},{\"id\":1,\"name\":\"US-MID\",\"data\":[{\"id\":81853,\"testType\":\"ping\",\"time\":\"20 Mar 2012 20:15:14 GMT\",\"perf\":33.286,\"status\":\"OK\",\"tag\":\"Default\",\"name\":\"google.com_ping\",\"frequency\":null,\"timeout\":600}],\"locationShortName\":\"US-MID\"},{\"id\":10,\"name\":\"US-WST\",\"data\":[{\"id\":81853,\"testType\":\"ping\",\"time\":\"20 Mar 2012 20:15:46 GMT\",\"perf\":3.043,\"status\":\"OK\",\"tag\":\"Default\",\"name\":\"google.com_ping\",\"frequency\":null,\"timeout\":600}],\"locationShortName\":\"US-WST\"},{\"id\":9,\"name\":\"US-EST\",\"data\":[{\"id\":81853,\"testType\":\"ping\",\"time\":\"20 Mar 2012 20:17:14 GMT\",\"perf\":3.541,\"status\":\"OK\",\"tag\":\"Default\",\"name\":\"google.com_ping\",\"frequency\":null,\"timeout\":600}],\"locationShortName\":\"US-EST\"}]";
            Structures.LocationWithMonitorsResults[] snapshot =
                Helper.DeserializeObject<Structures.LocationWithMonitorsResults[]>(testString,
                                                                                   output,
                                                                                   Params.result);
        }


        [Test]
        public void DeserializeObject_ExternalMonitor_GetTops_Json()
        {

        }


        [Test]
        public void DeserializeObject_ExternalMonitor_GetTops_Xml()
        {
            OutputType output = OutputType.XML;
            string testString =
                "<data>\r\n<tests>\r\n\t\t<test>\r\n\t\t<id>27278</id>\r\n\t\t<result>64.483</result>\r\n\t\t<testName>google.com_ping</testName>\r\n\t\t<lastCheckTime>12:41</lastCheckTime>\r\n\t\t<status>OK</status>\r\n\t</test>\r\n\t<test>\r\n\t\t<id>27278</id>\r\n\t\t<result>37.901</result>\r\n\t\t<testName>google.com_ping</testName>\r\n\t\t<lastCheckTime>12:34</lastCheckTime>\r\n\t\t<status>OK</status>\r\n\t</test>\r\n\t<test>\r\n\t\t<id>27278</id>\r\n\t\t<result>17.515</result>\r\n\t\t<testName>google.com_ping</testName>\r\n\t\t<lastCheckTime>12:41</lastCheckTime>\r\n\t\t<status>OK</status>\r\n\t</test>\r\n\t<test>\r\n\t\t<id>27278</id>\r\n\t\t<result>14.33</result>\r\n\t\t<testName>google.com_ping</testName>\r\n\t\t<lastCheckTime>12:41</lastCheckTime>\r\n\t\t<status>OK</status>\r\n\t</test>\r\n\t<test>\r\n\t\t<id>27278</id>\r\n\t\t<result>13.359</result>\r\n\t\t<testName>google.com_ping</testName>\r\n\t\t<lastCheckTime>12:41</lastCheckTime>\r\n\t\t<status>OK</status>\r\n\t</test>\r\n\t<test>\r\n\t\t<id>27278</id>\r\n\t\t<result>6.701</result>\r\n\t\t<testName>google.com_ping</testName>\r\n\t\t<lastCheckTime>12:37</lastCheckTime>\r\n\t\t<status>OK</status>\r\n\t</test>\r\n\t<test>\r\n\t\t<id>27278</id>\r\n\t\t<result>6.349</result>\r\n\t\t<testName>google.com_ping</testName>\r\n\t\t<lastCheckTime>12:41</lastCheckTime>\r\n\t\t<status>OK</status>\r\n\t</test>\r\n\t<test>\r\n\t\t<id>27278</id>\r\n\t\t<result>3.328</result>\r\n\t\t<testName>google.com_ping</testName>\r\n\t\t<lastCheckTime>12:41</lastCheckTime>\r\n\t\t<status>OK</status>\r\n\t</test>\r\n\t<test>\r\n\t\t<id>27278</id>\r\n\t\t<result>1.491</result>\r\n\t\t<testName>google.com_ping</testName>\r\n\t\t<lastCheckTime>12:37</lastCheckTime>\r\n\t\t<status>OK</status>\r\n\t</test>\r\n</tests>\r\n<tags>\r\n\t<tag>Default</tag>\r\n</tags>\r\n</data>";
            ExternalMonitorTop externalMonitorTop = Helper.DeserializeObject<ExternalMonitorTop>(testString, output,
                                                                                                 Params.data);
        }


        #endregion
    }
}
