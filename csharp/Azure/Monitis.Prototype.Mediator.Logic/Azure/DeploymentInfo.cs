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
        /// Get default deployment ID
        /// </summary>
        public static readonly string DefaultRoleInstanceName = Resources.RoleInstanceName;


        /// <summary>
        /// Unique identifier of deployment
        /// </summary>
        public String DeploymentID { get; set; }

        /// <summary>
        /// Gets or sets name of Role Instance
        /// </summary>
        public String RoleInstanceName { get; set; }
    }
}