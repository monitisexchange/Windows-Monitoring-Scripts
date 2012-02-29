using System;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Monitis.Prototype.Logic.Azure.Storage;

namespace Monitis.Prototype.Logic.Azure.TableService
{
    /// <summary>
    /// Represent context over table WADPerformanceCountersTable.
    /// This table contaims information about perfomance counters
    /// </summary>
    public class WADPerformanceTable : TableServiceContext
    {
        /// <summary>
        /// Azure table name
        /// </summary>
        public static readonly String TableName = StorageResource.WADPerformanceCountersTableName;

        public WADPerformanceTable(String baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
            ResolveType = ResolveEntityType;
        }

        public Type ResolveEntityType(String name)
        {
            Type type = typeof(PerformanceData);
            return type;
        }

        /// <summary>
        /// Represents queryable object over table context for LINQ queries
        /// </summary>
        public IQueryable<PerformanceData> Queryable
        {
            get { return CreateQuery<PerformanceData>(TableName); }
        }
    }
}
