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
            StorageAccountName = "azdemtest";
            StorageAccountKey =
                "cnI2Z4XY59CGePp5fA/0NFUzG8bt1u9Yq1xlwOjmzPTIc15WbCRMuAcr0DPIL7ATr/A3OEPiix5DXmEH+lJ8AQ==";
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