using System;
using Monitis.API.Common;

namespace Monitis.API.REST.User.Contract
{
    /// <summary>
    /// Response for get secretkey request
    /// </summary>
    public class SecretKeyResponce
    {
        /// <summary>
        /// Value of secrect key
        /// </summary>
        public String SecretKey { get; set; }
    }
}