using System;
using Microsoft.WindowsAzure.StorageClient;
using Monitis.Prototype.Logic.Azure.Storage.Analytics;
using NUnit.Framework;

namespace UnitTests.Azure
{
    /// <summary>
    /// Azure storage analytics test for Table service.
    /// NOTE: for blobs and queues the tests are same
    /// </summary>
    [TestFixture]
    public class TableStorageAnalyticsTests
    {
        [Test]
        public void GetAnalyticsSettingsTest()
        {
            CloudTableClient tableClient = new CloudTableClient(_credentials.TableBaseAddress, _credentials.StorageCredentials);
            AnalyticsSettings analyticsSettings = tableClient.GetServiceSettings();
            Assert.IsNotNull(analyticsSettings);
        }

        [Test]
        public void UpdateAnalyticsSettingsTest()
        {
            /*initialize settings for update*/
            AnalyticsSettings analyticsSettings = new AnalyticsSettings();
            analyticsSettings.IsLogRetentionPolicyEnabled = true;
            analyticsSettings.IsMetricsRetentionPolicyEnabled = true;

            //one day
            analyticsSettings.LogRetentionInDays = _oneDay.Days;
            analyticsSettings.LogType = LoggingLevel.Write | LoggingLevel.Read | LoggingLevel.Delete;
            analyticsSettings.LogVersion = AnalyticsSettings.Version;

            //one day
            analyticsSettings.MetricsRetentionInDays = _oneDay.Days;
            analyticsSettings.MetricsType = MetricsType.All;
            analyticsSettings.MetricsVersion = AnalyticsSettings.Version;

            CloudTableClient tableClient = new CloudTableClient(_credentials.TableBaseAddress, _credentials.StorageCredentials);
            tableClient.SetServiceSettings(analyticsSettings);
            AnalyticsSettings serviceSettings = tableClient.GetServiceSettings();
            Assert.IsNotNull(serviceSettings);
            Assert.AreEqual(analyticsSettings,serviceSettings);
        }

        #region private fields

        private readonly TestCredentials _credentials = new TestCredentials();

        /// <summary>
        /// One day time span
        /// </summary>
        private TimeSpan _oneDay = new TimeSpan(1,0,0,0);

        #endregion private fields
    }
}