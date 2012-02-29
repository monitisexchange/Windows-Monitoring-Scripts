using System;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Monitis.Prototype.Logic.Azure.Storage.Tables.MetricsTransactions
{
    public class MetricsTransactionsQuery
    {
        public MetricsTransactionsEntity[] GetMetrics(CloudStorageAccount storageAccount, DateTime startDateTimeUTC, DateTime endDateTimeUTC)
        {
            MetricsTransactionsTable table = new MetricsTransactionsTable(storageAccount,
                                                                          storageAccount.Credentials);
            //convert datetimes to partition keys
            String startingPartitionKey = startDateTimeUTC.ToString(StorageResource.TransactionPrimaryKeyTimeFormat);
            String endingPartitionKey = endDateTimeUTC.ToString(StorageResource.TransactionPrimaryKeyTimeFormat);

            var query = from transactionsEntity in table.Queryable
                        where transactionsEntity.PartitionKey.CompareTo(startingPartitionKey) >= 0
                              && transactionsEntity.PartitionKey.CompareTo(endingPartitionKey) <= 0
                        select transactionsEntity;

            CloudTableQuery<MetricsTransactionsEntity> tableServiceQuery = query.AsTableServiceQuery();

            MetricsTransactionsEntity[] metricsTransactionsEntities;

            try
            {
                //execute query
                metricsTransactionsEntities = tableServiceQuery.Execute().ToArray();
            }
            catch (Exception exception)
            {
                //TODO:log
                throw;
            }

            return metricsTransactionsEntities;
        }
    }
}