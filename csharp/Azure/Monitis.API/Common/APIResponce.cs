using System;

namespace Monitis.API.Common
{
    /// <summary>
    /// Base type for response  with common properties
    /// </summary>
    public class APIResponce
    {
        /// <summary>
        /// Error message
        /// </summary>
        public String Error { get; set; }

        /// <summary>
        /// Status message
        /// </summary>
        public String Status { get; set; }

        /// <summary>
        /// True - status message is ok
        /// </summary>
        public Boolean IsOk
        {
            get
            {
                if(String.IsNullOrEmpty(Status))
                {
                    return false;
                }
                return Status.ToLower().Equals("ok");
            }
        }
    }
}