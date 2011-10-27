using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Http;
using System.Xml.Linq;

namespace MonitisTop
{
    /// <summary>
    /// Base monitor class
    /// </summary>
    public class Monitor
    {
        protected Agent agent;

        protected string id;
        protected string name;
        protected int maxMetricWidth;
        protected List<Metric> metrics;
        protected List<MonitorDefinition> monitorsDefinitions;

        public string Id { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }
        public int MaxMetricWidth { get { return maxMetricWidth; } set { maxMetricWidth = value; } }
        public List<Metric> Metrics { get { return metrics; } set { metrics = value; } }

        public Monitor(Agent _agent)
        {
            agent = _agent;
            maxMetricWidth = 0;

            metrics = new List<Metric>();
            monitorsDefinitions = new List<MonitorDefinition>();
        }


        // Abstract function to retrieve the metrics for a monitor
        public virtual void GetMetrics(string apiKey, HttpClient client) {}


        /// <summary>
        /// Add a metric to the list of metrics for this monitor and keep track of the 
        /// maximum display width of the metric name
        /// </summary>
        /// <param name="_metric"></param>
        protected virtual void AddMetric(Metric _metric)
        {
            if (_metric.Name.Length > maxMetricWidth)
                maxMetricWidth = _metric.Name.Length;

            metrics.Add(_metric);
        }


        /// <summary>
        /// Output the monitor results to the screen
        /// </summary>
        /// <param name="maxWidth"></param>
        public virtual void DisplayMonitor()
        {
            // write the monitor name
            Console.Write("{0}", Util.Pad(this.Name, agent.MaxMonitorWidth + 4));

            // write the metrics headers for this monitor
            foreach (Metric metric in this.metrics)
            {
                metric.DisplayMetricHeader();
            }
            Console.Write("\n{0}", Util.Pad(" ", agent.MaxMonitorWidth + 4));

            // write the metric results
            foreach (Metric metric in this.Metrics)
            {
                metric.DisplayMetricResults();
            }
            Console.Write("\n\n");
        }
    }
}
