using System;
using Microsoft.WindowsAzure;

namespace UnitTests.Azure
{
    /// <summary>
    /// Contains credentials for testing Azure requests
    /// </summary>
    public class TestCredentials
    {
        #region constrcutors

        /// <summary>
        /// Default constructor with default credentials initialization
        /// </summary>
        public TestCredentials()
        {
            StorageAccountName = "azdem";
            StorageAccountKey =
                "q4W4vqQATcVq+EqCDbuG5TwExkMTD5YmRQ4IRW8R0Wk0H2GOF2leRHFy2I2EqF/C22Oya4TeqwYawfOVTgbz2A==";
        }

        #endregion constrcutors

        #region public properties

        public String StorageAccountName { get; set; }

        public String StorageAccountKey { get; set; }

        public String TableBaseAddress
        {
            get
            {
                return String.Format("https://{0}.table.core.windows.net", StorageAccountName);
            }
        }

        public StorageCredentialsAccountAndKey StorageCredentials
        {
            get
            {
                if (_storageCredentials == null)
                {
                    _storageCredentials = new StorageCredentialsAccountAndKey(StorageAccountName, StorageAccountKey);
                }
                return _storageCredentials;
            }
        }

        #endregion public properties

        #region private fields

        private StorageCredentialsAccountAndKey _storageCredentials;

        #endregion
    }
}