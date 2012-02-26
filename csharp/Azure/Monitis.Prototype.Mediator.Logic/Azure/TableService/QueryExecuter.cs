using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Monitis.Prototype.Logic.Azure.TableService
{
    /// <summary>
    /// Query helper for retrieving data from the WADPerformanceCountersTable
    /// </summary>
    public class QueryExecuter
    {
        /// <summary>
        /// Cloud storage account client
        /// </summary>
        private readonly CloudStorageAccount _accountStorage;

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
        public QueryExecuter(string accountName, string privateKey)
        {
            _accountStorage = CloudStorageAccount.Parse(String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", accountName, privateKey));
        }

        /// <summary>
        /// Retrive Performance counter data
        /// </summary>
        /// <param name="counterFullName">Perfomance counter specifier full name</param>
        /// <param name="roleInstanceName">Deployment id</param>
        /// <param name="startPeriod">Start sample date time</param>
        /// <param name="endPeriod">End sample date time</param>
        /// <returns></returns>
        public List<PerformanceData> GetPerformanceCounters(String counterFullName, String roleInstanceName, DateTime startPeriod, DateTime endPeriod)
        {
            //create context for WAD table
            WADPerformanceTable context = new WADPerformanceTable(_accountStorage.TableEndpoint.ToString(), _accountStorage.Credentials);

            IQueryable<PerformanceData> data = context.Data;

            CloudTableQuery<PerformanceData> query = null;

            //query for pefomance counters
            query = (from row in data
                     where row.CounterName == counterFullName
                        && row.EventTickCount >= startPeriod.Ticks
                        && row.EventTickCount <= endPeriod.Ticks
                        && row.RoleInstance.Equals(roleInstanceName)
                     select row).AsTableServiceQuery();

            List<PerformanceData> selectedData = new List<PerformanceData>();
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
    }
}