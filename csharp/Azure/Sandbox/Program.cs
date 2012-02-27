using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Monitis.Prototype.Logic.Azure.Storage.Analytics;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                String accountName = "azdem";
                String accountKey = "q4W4vqQATcVq+EqCDbuG5TwExkMTD5YmRQ4IRW8R0Wk0H2GOF2leRHFy2I2EqF/C22Oya4TeqwYawfOVTgbz2A==";
                String tableBaseAddress = "https://azdem.table.core.windows.net";
                StorageCredentialsAccountAndKey storageCredentials = new StorageCredentialsAccountAndKey(accountName, accountKey);
                Microsoft.WindowsAzure.StorageClient.CloudTableClient tableClient = new CloudTableClient(tableBaseAddress, storageCredentials);
                //tableClient.
                AnalyticsSettings analyticsSettings = Monitis.Prototype.Logic.Azure.Storage.Analytics.AnalyticsSettingsExtension.GetServiceSettings(tableClient);
            }
            catch (Exception exception)
            {
                
                
            }
            

        }
    }
}
