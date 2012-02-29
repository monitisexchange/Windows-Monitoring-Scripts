using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Monitis.Prototype.Logic.Azure.Storage.Tables.MetricsTransactions;

namespace Monitis.Prototype.Logic.Azure.TableService
{
    /// <summary>
    /// Query helper for retrieving data from the Windows Azure Table service
    /// </summary>
    public class QueryExecuter
    {
        #region constructors

        /// <summary>
        /// Default Constructor - Use development storage emulator.
        /// </summary>
        public QueryExecuter()
        {
            _accountStorage = CloudStorageAccount.DevelopmentStorageAccount;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="accountName">Azure storage name</param>
        /// <param name="privateKey">Azure storage private key</param>
        public QueryExecuter(String accountName, String privateKey)
        {
            _accountStorage = CloudStorageAccount.Parse(String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", accountName, privateKey));
        }

        #endregion

        #region public methods

        /// <summary>
        /// Retrive Performance counter data
        /// </summary>
        /// <param name="counterFullName">Perfomance counter specifier full name</param>
        /// <param name="roleInstanceName">Deployment id</param>
        /// <param name="startPeriod">Start sample date time</param>
        /// <param name="endPeriod">End sample date time</param>
        /// <returns></returns>
        public IEnumerable<PerformanceData> GetPerformanceCounters(String counterFullName, String roleInstanceName, DateTime startPeriod, DateTime endPeriod)
        {
            //create context for WAD table
            WADPerformanceTable context = new WADPerformanceTable(_accountStorage.TableEndpoint.ToString(), _accountStorage.Credentials);

            //query for pefomance counters
            CloudTableQuery<PerformanceData> query = (from row in context.Queryable
                                                      where row.CounterName == counterFullName
                                                         && row.EventTickCount >= startPeriod.Ticks
                                                         && row.EventTickCount <= endPeriod.Ticks
                                                         && row.RoleInstance.Equals(roleInstanceName)
                                                      select row).AsTableServiceQuery();
           
            List<PerformanceData> selectedData;
            try
            {
                selectedData = query.Execute().ToList();
            }
            catch (Exception exception)
            {
                //TODO: log
                throw;
            }
            return selectedData;
        }

        public MetricsTransactionsEntity[] GetMetricData(DateTime startPeriod, DateTime endPeriod)
        {
            MetricsTransactionsQuery transactionsQuery = new MetricsTransactionsQuery();
            return transactionsQuery.GetMetrics(_accountStorage, startPeriod, endPeriod);
        }

        #endregion

        #region private fields

        /// <summary>
        /// Cloud storage account client
        /// </summary>
        private readonly CloudStorageAccount _accountStorage;

        #endregion private fields
    }
}