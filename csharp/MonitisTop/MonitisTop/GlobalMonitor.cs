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
    /// Global monitor class
    /// </summary>
    public class GlobalMonitor : Monitor
    {
        public GlobalMonitor(Agent _agent)
            : base(_agent)
        {
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
            sm = new MonitorDefinition("cpu");
            sm.AddNamePair("user_value", "%");
            sm.AddNamePair("kernel_value", "%");
            tmp.Add(sm);

            sm = new MonitorDefinition("drive");
            sm.AddNamePair("result", "MB");
            tmp.Add(sm);

            sm = new MonitorDefinition("memory");
            sm.AddNamePair("free_memory", "MB");
            sm.AddNamePair("total_memory", "MB");
            tmp.Add(sm);

            return tmp;
        }


        public override void GetMetrics(string apiKey, HttpClient client)
        {
            // Get the 'base' name of the monitor to make sure we requesting the correct monitor results. (drive_C becomes: drive)
            string baseMonitorName = Util.BaseMonitorName(this.Name, '_');

            // Post the requests
            using (HttpResponseMessage response = client.Get("api?action=top" + baseMonitorName + "&apikey=" + apiKey + "&output=xml&detailedResults=true"))
            {
                response.EnsureStatusIsSuccessful();

                String data = response.Content.ReadAsString();
                XDocument xml = XDocument.Parse(data);

                IEnumerable<XElement> allMetrics = null;
                foreach (MonitorDefinition sm in monitorsDefinitions)
                {
                    allMetrics = from metricNode in xml.Descendants("test")
                                 where (string)metricNode.Element("id") == this.Id
                                 select metricNode;

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
