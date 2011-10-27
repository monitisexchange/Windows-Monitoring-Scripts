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
    public class ExternalMonitor : Monitor
    {
        public ExternalMonitor(Agent _agent)
            : base(_agent)
        {
            // Add the supported monitors 
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
            sm = new MonitorDefinition("http");
            sm.AddNamePair("memUsage", "MB");
            sm.AddNamePair("cpuUsage", "%");
            tmp.Add(sm);

            return tmp;
        }


        /// <summary>
        /// Get the metrics for this monitor
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="client"></param>
        public override void GetMetrics(string apiKey, HttpClient client)
        {
            base.GetMetrics(apiKey, client);

            // Get the serial date
            DateTime dt = new DateTime();
            dt = DateTime.Now;

            // Send the http request
            using (HttpResponseMessage response = client.Get("api?action=testresult&apikey=" + apiKey + "&output=xml&testId=" + this.Id + "&day=" + dt.Day + "&month=" + dt.Month + "&year=" + dt.Year))
            {
                response.EnsureStatusIsSuccessful();

                String data = response.Content.ReadAsString();
                XDocument xml = XDocument.Parse(data);

                IEnumerable<XElement> allMetrics = null;
                allMetrics = from metricNode in xml.Descendants("location")
                             select metricNode;

                // Enumerate the metrics and store them in their own Metric object
                foreach (XElement xe in allMetrics)
                    if (xe.Attribute("name") != null)
                    {
                        Metric m = new Metric(this);
                        m.Name = xe.Attribute("name").Value;

                        foreach (XElement node in xe.Elements("row"))
                        {
                            foreach (XElement el in node.Elements("cell"))
                                m.Result = el.Value;

                            m.Suffix = "";
                        }
                        this.AddMetric(m);
                    }
            }
        }
    }
}
