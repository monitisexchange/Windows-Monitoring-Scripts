using System;

namespace Monitis.API.Common
{
    /// <summary>
    /// Contain API actions names
    /// </summary>
    public static class ActionNames
    {
        /// <summary>
        ///  Get authentication token
        /// </summary>
        public static String AuthToken
        {
            get { return "authToken"; }
        }

        /// <summary>
        /// Get user's all Custom monitors
        /// </summary>
        public static String GetMonitors
        {
            get { return "getMonitors"; }
        }

        /// <summary>
        /// Add a new Custom monitor
        /// </summary>
        public static String AddMonitor
        {
            get { return "addMonitor"; }
        }

        /// <summary>
        ///  Add results for the specified Custom monitor
        /// </summary>
        public static String AddResult
        {
            get { return "addResult"; }
        }

        /// <summary>
        /// Get secret key 
        /// </summary>
        public static String SecretKey
        {
            get { return "secretkey"; }
        }
    }
}