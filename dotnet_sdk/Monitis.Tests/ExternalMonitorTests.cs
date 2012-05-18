using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Monitis.Tests
{
    [TestFixture]
    public class ExternalMonitorTests
    {
        private const string tagNew = "tag new";
        private Authentication authentication = null;
        private ExternalMonitor externalMonitor = null;

        private List<int> monitorsToDelete = new List<int>();

        [TestFixtureSetUp]
        public void Setup()
        {
            authentication = new Authentication(apiKey: MonitisAccountInformation.ApiKey,
                                                secretKey: MonitisAccountInformation.SekretKey);

            externalMonitor = new ExternalMonitor();
            externalMonitor.SetAuthenticationParams(authentication);
        }

        [TearDown]
        public void TearDown()
        {
            if (monitorsToDelete.Count > 0)
            {
                externalMonitor.DeleteMonitors(monitorsToDelete.ToArray());
                monitorsToDelete.Clear();
            }
        }

        #region GetMonitors

        [Test]
        public void GetMonitors_Xml()
        {
            GetMonitors(output: OutputType.XML);
        }

        [Test]
        public void GetMonitors_Json()
        {
            GetMonitors(output: OutputType.JSON);
        }

        public void GetMonitors(OutputType output)
        {
            var result = externalMonitor.GetMonitors(output);
        }

        #endregion

        #region AddMonitor

        [Test]
        public void AddMonitor_AddMonitorAndDelete_Xml()
        {
            AddMonitorAndDelete(output: OutputType.XML);
        }

        [Test]
        public void AddMonitor_AddMonitorAndDelete_Json()
        {
            AddMonitorAndDelete(output: OutputType.JSON);
        }

        public void AddMonitorAndDelete(OutputType output)
        {
            int idMonitorToDelete = AddMonitor(externalMonitor, output);
            monitorsToDelete.Add(idMonitorToDelete);
        }

        private const string url = "www.yandex.ru";

        public static int AddMonitor(ExternalMonitor externalMonitor, OutputType output,bool dnsWithParams=false)
        {
            string testName = "test ping ololo: ololo;" + Common.GenerateRandomString(5);
             string tag = tagNew;
            int[] locationsIds = new int[] { 4, 11 };
            ExternalMonitor.TestType testType = ExternalMonitor.TestType.ping;
            if (dnsWithParams)
                testType = ExternalMonitor.TestType.dns;

            var testparams = new Dictionary<string, string>();
            if (dnsWithParams)
            {
                //testparams.Add("test1", "fffgfygfhgf");
                //testparams.Add("test2", "bbb");
                //testparams.Add("test3", "dddd");
                //testparams.Add("test4", "ffff");
                //testparams.Add("test5", "ffff");

                testparams.Add("server", "google.com");
                testparams.Add("expip", "209.85.148.113");
                testparams.Add("expauth", "A");
            }

            var result = externalMonitor.AddMonitor(
                testType,
                testName,
                url,
                ExternalMonitor.CheckInterval.five,
                tag,
                output: output,
                locationIds: locationsIds,
                testParams: testparams);
            return result.testId;
        }

        [Test]
        public void AddMonitor_AddMonitorDnsWithParamsAndDelete_Xml()
        {
            AddMonitorDnsWithParamsAndDelete(output: OutputType.XML);
        }

        [Test]
        public void AddMonitor_AddMonitorDnsWithParamsAndDelete_Json()
        {
            AddMonitorDnsWithParamsAndDelete(output: OutputType.JSON);
        }

        public void AddMonitorDnsWithParamsAndDelete(OutputType output)
        {
            int idMonitorToDelete = AddMonitor(externalMonitor, output,true);
            monitorsToDelete.Add(idMonitorToDelete);
        }

        #endregion

        #region EditMonitor

        [Test]
        public void EditMonitor_AddMonitorEditAndDelete_Xml()
        {
            EditMonitor_AddMonitorEditAndDelete(output: OutputType.XML);
        }

        [Test]
        public void EditMonitor_AddMonitorEditAndDelete_Json()
        {
            EditMonitor_AddMonitorEditAndDelete(output: OutputType.JSON);
        }

        public void EditMonitor_AddMonitorEditAndDelete(OutputType output)
        {
            //Add monitor
            string testName = "test ping" + Common.GenerateRandomString(5);
            string newName = "new name";/*edit name*/

            string url = "www.yandex.ru";
            string tag = tagNew;

            int[] locationsIds = new int[] { 4, 11 };
            var locationsIdAndIntrvals = new Dictionary<int, ExternalMonitor.CheckInterval>();

            //edit monitor id (4 -> 5)
            locationsIdAndIntrvals.Add(5,ExternalMonitor.CheckInterval.five);
            locationsIdAndIntrvals.Add(11, ExternalMonitor.CheckInterval.fifteen);

            var resultAddMonitor = externalMonitor.AddMonitor(
                ExternalMonitor.TestType.ping,
                testName,
                url,
                ExternalMonitor.CheckInterval.five,
                tag,
                output: output,
                locationIds: locationsIds);

            monitorsToDelete.Add(resultAddMonitor.testId);

            int uptimeSLA = 10;
            //Edit monitor
            externalMonitor.EditMonitor(resultAddMonitor.testId, name: newName, url: url, locationIdIntervals: locationsIdAndIntrvals,
                                        tag: tag,uptimeSLA:uptimeSLA, output: output);
        }

         #endregion

        #region GetTags

        [Test]
        public void GetTags_AddMonitorGetTagsDeleteMonitor_Json()
        {
            GetTags(output: OutputType.JSON);
        }

         [Test]
        public void GetTags_AddMonitorGetTagsDeleteMonitor_XML()
        {
            GetTags(output: OutputType.XML);
        }

        public void GetTags(OutputType output)
        {
            int idMonitorToDelete = AddMonitor(externalMonitor, output);
            monitorsToDelete.Add(idMonitorToDelete);

            externalMonitor.GetTags(output: output);
        }

        #endregion

        #region GetTagTests

        [Test]
        public void GetTagTests_AddMonitorGetTagsDeleteMonitor_Json()
        {
            GetTagTests(output: OutputType.JSON);
        }

        [Test]
        public void GetTagTests_AddMonitorGetTagsDeleteMonitor_XML()
        {
            GetTagTests(output: OutputType.XML);
        }

        public void GetTagTests(OutputType output)
        {
            int idMonitorToDelete = AddMonitor(externalMonitor, output);
            monitorsToDelete.Add(idMonitorToDelete);

            externalMonitor.GetTagTests(tagNew,output: output);
        }

        #endregion

        #region GetLocations

        [Test]
        public void GetLocations_Json()
        {
            GetLocations(output: OutputType.JSON);
        }

        [Test]
        public void GetLocations_XML()
        {
            GetLocations(output: OutputType.XML);
        }

        public void GetLocations(OutputType output)
        {
            externalMonitor.GetLocations(output: output);
        }

        #endregion

        #region GetSnapshot

        [Test]
        public void GetSnapshot_Json()
        {
            GetSnapshot(output: OutputType.JSON);
        }

        [Test]
        public void GetSnapshot_XML()
        {
            GetSnapshot(output: OutputType.XML);
        }

        public void GetSnapshot(OutputType output)
        {
           Structures.LocationWithMonitorsResults[] snapshot= externalMonitor.GetSnapshot(output: output);
        }

        #endregion

        #region GetMonitorInfo

        [Test]
        public void GetMonitorInfo_AddMonitorGetMonitorInfoDeleteMonitor_Json()
        {
            GetMonitorInfo(output: OutputType.JSON);
        }

        [Test]
        public void GetMonitorInfo_AddMonitorGetMonitorInfoDeleteMonitor_XML()
        {
            GetMonitorInfo(output: OutputType.XML);
        }

        public void GetMonitorInfo(OutputType output)
        {
            int monitorID = output == OutputType.JSON ? AddMonitor(externalMonitor, output, true) : AddMonitor(externalMonitor, output);
            monitorsToDelete.Add(monitorID);
            var monitorInfo = externalMonitor.GetMonitorInfo(monitorId: monitorID, output: output);
        }

        #endregion

        #region GetTops

        [Test]
        public void GetTops_Json()
        {
            GetTops(output: OutputType.JSON);
        }

        [Test]
        public void GetTops_XML()
        {
            GetTops(output: OutputType.XML);
        }

        public void GetTops(OutputType output)
        {
            var top = externalMonitor.GetTops(output: output);
        }

        #endregion

        #region SuspendMonitors

        [Test]
        public void SuspendMonitors_SuspendById_Json()
        {
            SuspendMonitorsSuspendById(output: OutputType.JSON);
        }

        [Test]
        public void SuspendMonitors_SuspendById_XML()
        {
            SuspendMonitorsSuspendById(output: OutputType.XML);
        }

        public void SuspendMonitorsSuspendById(OutputType output)
        {
            int idMonitorToDelete = AddMonitor(externalMonitor, output);
            monitorsToDelete.Add(idMonitorToDelete);

            externalMonitor.SuspendMonitors(idMonitorToDelete,output);
        }

        [Test]
        public void SuspendMonitors_SuspendByTag_Json()
        {
            SuspendMonitorsSuspendById(output: OutputType.JSON);
        }

        [Test]
        public void SuspendMonitors_SuspendByTag_XML()
        {
            SuspendMonitorsSuspendById(output: OutputType.XML);
        }

        public void SuspendMonitorsSuspendTagId(OutputType output)
        {
            int idMonitorToDelete = AddMonitor(externalMonitor, output);
            monitorsToDelete.Add(idMonitorToDelete);

            externalMonitor.SuspendMonitors(tag:tagNew,output:output);
        }


        #endregion

        #region ActivateMonitors

        [Test]
        public void ActivateMonitors_ActivateById_Json()
        {
            ActivateMonitors_ActivateById(output: OutputType.JSON);
        }

        [Test]
        public void ActivateMonitors_ActivateById_XML()
        {
            ActivateMonitors_ActivateById(output: OutputType.XML);
        }

        public void ActivateMonitors_ActivateById(OutputType output)
        {
            int idMonitorToDelete = AddMonitor(externalMonitor, output);
            monitorsToDelete.Add(idMonitorToDelete);

            externalMonitor.ActivateMonitors(idMonitorToDelete,output);
        }

        [Test]
        public void ActivateMonitors_ActivateByTag_Json()
        {
            ActivateMonitors_ActivateByTag(output: OutputType.JSON);
        }

        [Test]
        public void ActivateMonitors_ActivateByTag_XML()
        {
            ActivateMonitors_ActivateByTag(output: OutputType.XML);
        }

        public void ActivateMonitors_ActivateByTag(OutputType output)
        {
            int idMonitorToDelete = AddMonitor(externalMonitor, output);
            monitorsToDelete.Add(idMonitorToDelete);

            externalMonitor.ActivateMonitors(tag:tagNew,output:output);
        }

        #endregion

        #region GetMonitorResults

        [Test]
        [Ignore("error in response")]
        public void GetMonitorResults_Json()
        {
            GetMonitorResults(output: OutputType.JSON);
        }

        [Test]
        [Ignore("error in response")]
        public void GetMonitorResults_XML()
        {
            GetMonitorResults(output: OutputType.XML);
        }

        public void GetMonitorResults(OutputType output)
        {
            DateTime dt = DateTime.Now;

            var monitors = externalMonitor.GetMonitors(output: output);
            int monitorId = monitors.First().id;

            externalMonitor.GetMonitorResults(monitorId, dt.Year, dt.Month, 17,timezone:360, output: output,locationIds:4);
        }

        #endregion
    }
}
