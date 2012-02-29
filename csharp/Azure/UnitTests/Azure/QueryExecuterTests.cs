using System;
using System.Diagnostics;
using Monitis.Prototype.Logic.Azure.Storage.Tables.MetricsTransactions;
using Monitis.Prototype.Logic.Azure.TableService;
using NUnit.Framework;

namespace UnitTests.Azure
{
    /// <summary>
    /// Tests for <see cref="QueryExecuter"/> class over tables
    /// </summary>
    [TestFixture]
    public class QueryExecuterTests
    {
        [Test]
        public void GetTableTransactionsTest()
        {
            //NOTE: this test pass if azure service already contains any transaction data for last 24 hours period
            //for setup metrics collection configure analytics settings

            QueryExecuter queryExecuter = new QueryExecuter(_testCredentials.StorageAccountName, _testCredentials.StorageAccountKey);
            MetricsTransactionsEntity[] metricsTransactionsEntities = queryExecuter.GetMetricData(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            Assert.Greater(metricsTransactionsEntities.Length, 0);
            Assert.IsNotNull(metricsTransactionsEntities);
        }

        private TestCredentials _testCredentials = new TestCredentials();

    }
}