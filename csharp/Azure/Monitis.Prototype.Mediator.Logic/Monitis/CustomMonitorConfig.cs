using System;
using Monitis.API.Domain.Monitors;

namespace Monitis.Prototype.Logic.Monitis
{
    public class CustomMonitorConfig
    {
        public CustomMonitorConfig(MonitorDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }

            Descriptor = descriptor;
            Name = Descriptor.Name;
        }

        public String Name { get; private set; }

        public String SourceMetricName { get; set; }

        public MonitorDescriptor Descriptor { get; private set; }

        public Int32 MonitorID { get; set; }
    }
}