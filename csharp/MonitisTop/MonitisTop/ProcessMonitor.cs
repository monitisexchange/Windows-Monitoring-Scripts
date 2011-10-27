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
    /// Process Monitor class
    /// </summary>
    public class ProcessMonitor : Monitor
    {
        public ProcessMonitor(Agent _agent)
            : base(_agent)
        {
            // Get the supported monitors 
            monitorsDefinitions.AddRange(GetMonitorDefinitions());
        }


        /// <summary>
        /// Return the list of supported Monitors
        /// </summary>
        /// <returns></returns>
        public static List<MonitorDefinition> GetMonitorDefinitions()
        {
            List<MonitorDefinition> tmp = new List<MonitorDefinition>();

            MonitorDefinition sm;
            sm = new MonitorDefinition("process");
            sm.AddNamePair("memory_usage", "MB");
            sm.AddNamePair("cpu_usage", "%");
            tmp.Add(sm);

            return tmp;
        }


        /// <summary>
        /// Get the results for the process monitor
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="client"></param>
        public override void GetMetrics(string apiKey, HttpClient client)
        {
            base.GetMetrics(apiKey, client);

            using (HttpResponseMessage response = client.Get("api?action=topProcessByCPUUsage&limit=50&apikey=" + apiKey + "&output=xml&detailedResults=true"))
            {
                response.EnsureStatusIsSuccessful();

                String data = response.Content.ReadAsString();
                XDocument xml = XDocument.Parse(data);

                IEnumerable<XElement> allMetrics = null;
                foreach (MonitorDefinition sm in monitorsDefinitions)
                {
                    // Query the xml data to retrieve each test result matching the ID of the current monitor
                    allMetrics = from metricNode in xml.Descendants("test")
                                 where (string)metricNode.Element("id") == this.Id
                                 select metricNode;

                    // Enumerate the metrics and store them in their own Metric object
                    foreach (XElement xe in allMetrics)
                        foreach (string s in sm.Names)
                            if (xe.Element(s) != null)
                            {
                                Metric m = new Metric(this);
                                m.Name = s;
                                m.Result = xe.Element(s).Value;
                                m.Suffix = sm.Suffixes[sm.Names.IndexOf(s)];
                                this.AddMetric(m);
                            }
                }
            }
        }
    }

}
