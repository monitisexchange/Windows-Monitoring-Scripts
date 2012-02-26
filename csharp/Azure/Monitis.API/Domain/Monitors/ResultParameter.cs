using System;

namespace Monitis.API.Domain.Monitors
{
    /// <summary>
    /// Result of concrete parameter
    /// </summary>
    public class ResultParameter
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Parameter value
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// Value timestamp 
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}