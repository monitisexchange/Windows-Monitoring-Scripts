using System;

namespace Monitis.Prototype.Logic.Azure
{
    /// <summary>
    /// Represents data about Windows Azure Compute deployment
    /// </summary>
    public class DeploymentInfo
    {
        /// <summary>
        /// Get default deployment ID
        /// </summary>
        public static readonly string DefaultDeploymentID = Resources.DefaultDeploymentID;

        /// <summary>
        /// Unique identifier of deployment
        /// </summary>
        public String DeploymentID { get; set; }

    }
}