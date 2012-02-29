using System;
using Microsoft.WindowsAzure.StorageClient;

namespace Monitis.Prototype.Logic.Azure.Storage.Tables.MetricsTransactions
{
    /// <summary>
    /// Entity describe structure of table <see cref="MetricsTransactionsTable"/>
    /// </summary>
    public class MetricsTransactionsEntity : TableServiceEntity
    {
        #region public properties

        /// <summary>
        /// The number of requests made to a storage service or the specified API operation.
        /// This number includes successful and failed requests, as well as requests which produced errors.
        /// </summary>
        public Int64 TotalRequests { get; set; }

        /// <summary>
        /// The number of billable requests.
        /// </summary>
        public Int64 TotalBillableRequests { get; set; }

        /// <summary>
        /// The percentage of availability for the storage service or the specified API operation.
        /// Availability is calculated by taking the TotalBillableRequests value and dividing it by the number of applicable requests,
        /// including those that produced unexpected errors. All unexpected errors result in reduced availability for the storage service or the specified API operation.
        /// </summary>
        public Double Availability { get; set; }

        #endregion public properties
    }
}