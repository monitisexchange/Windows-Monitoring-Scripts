using System;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Monitis.Prototype.Logic.Azure.Storage.Tables.MetricsTransactions
{
    public class MetricsTransactionsTable : TableServiceContext
    {
        public static String TableName = StorageResource.MetricsTransactionsTableName;

        #region constructor

        public MetricsTransactionsTable(CloudStorageAccount storageAccount, StorageCredentials credentials)
            : base(storageAccount.TableEndpoint.AbsoluteUri, credentials)
        {
            ResolveType = this.ResolveEntityType;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Resolve type of entity over table
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Type ResolveEntityType(String name)
        {
            Type type = typeof(MetricsTransactionsEntity);
            return type;
        }

        #endregion

        #region public properteis

        /// <summary>
        /// Represents queryable object over table context for LINQ queries
        /// </summary>
        public IQueryable<MetricsTransactionsEntity> Queryable
        {
            get
            {
                return base.CreateQuery<MetricsTransactionsEntity>(TableName);
            }
        }

        #endregion
    }
}