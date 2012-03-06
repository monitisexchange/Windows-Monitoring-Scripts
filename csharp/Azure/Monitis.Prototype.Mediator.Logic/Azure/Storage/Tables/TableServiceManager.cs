using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Monitis.Prototype.Logic.Azure.TableService
{
    /// <summary>
    /// Class represents methods for connect and manage Azure Table service
    /// </summary>
    public class TableServiceManager
    {
        /// <summary>
        ///  Default storage account name
        /// </summary>
        public static readonly String DefaultAccountName = Resources.DefaultStorageAccountName;

        /// <summary>
        /// Default storage account key
        /// </summary>
        public static String DefaultAccountKey = Resources.DefaultStorageAccountKey;
            
        /// <summary>
        /// Connection by default
        /// </summary>
        private static readonly CloudStorageAccount DefaultCredentials = new CloudStorageAccount(new StorageCredentialsAccountAndKey(DefaultAccountName, DefaultAccountKey), true);

        /// <summary>
        /// Try connect to Azure Table Service. Use for check connection
        /// </summary>
        /// <param name="accountName">Storage account name</param>
        /// <param name="key">Storage account key</param>
        /// <returns></returns>
        public static Boolean TryConnect(String accountName, String key)
        {
            if (String.IsNullOrEmpty(accountName))
            {
                throw new ArgumentNullException("accountName");
            }

            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            CloudTableClient cloudTableClient = GetClient(accountName, key);
            Boolean isCanConnect = false;
            try
            {
                //try list tables for test connection
                cloudTableClient.Timeout = new TimeSpan(0, 0, 0, 30);
                string listTables = cloudTableClient.ListTables().FirstOrDefault();
                isCanConnect = true;
            }
            catch (Exception exception)
            {
                //TODO: log
            }
            return isCanConnect;
        }

        /// <summary>
        /// Try connect to default storage account
        /// </summary>
        /// <returns></returns>
        public static Boolean TryConnectDefault()
        {
            return DefaultCredentials.Credentials.CanSignRequest;
        }

        /// <summary>
        /// Get table client
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static CloudTableClient GetClient(String accountName, String key)
        {
            CloudStorageAccount account = new CloudStorageAccount(new StorageCredentialsAccountAndKey(accountName, key), true);
            return account.CreateCloudTableClient();
        }

        /// <summary>
        /// Cehck is table<see cref="WADPerformanceTable"/>exists in storage service
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Boolean IsPerfomanceTableExists(String accountName, String key)
        {
            CloudTableClient cloudTableClient = GetClient(accountName, key);
            return cloudTableClient.DoesTableExist(WADPerformanceTable.TableName);
        }
    }
}