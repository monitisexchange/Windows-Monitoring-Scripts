using System;

namespace Monitis.API.Domain.Monitors
{
    /// <summary>
    /// Monitis monitor
    /// </summary>
    public class Monitor
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public Int32 ID { get; set; }

        /// <summary>
        /// Monitor name
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        public String Tag { get; set; }

        /// <summary>
        /// Array of parameters names
        /// </summary>
        public String[] MonitorParams { get; set; }

        /// <summary>
        /// Type of monitor
        /// </summary>
        public String Type { get; set; }
    }
}