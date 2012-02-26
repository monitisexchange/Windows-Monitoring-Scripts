using System;
using Monitis.Prototype.Logic.Azure;

namespace Monitis.Prototype.Logic.Common
{
    /// <summary>
    /// Windows azure info
    /// </summary>
    public class AzureInfo
    {
        /// <summary>
        /// Account name
        /// </summary>
        public String AccountName { get; set; }

        /// <summary>
        /// Account key
        /// </summary>
        public String AccountKey { get; set; }

        /// <summary>
        /// Deployment info
        /// </summary>
        public DeploymentInfo DeploymentInfo { get; set; }

        /// <summary>
        /// Gets or sets deployed role instance name 
        /// </summary>
        public String RoleInstanceName { get; set; }
    }
}
