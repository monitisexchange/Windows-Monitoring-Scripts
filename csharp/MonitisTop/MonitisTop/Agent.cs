using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Http;
using System.Xml.Linq;

namespace MonitisTop
{
    public class Agent
    {
        private string id;
        private string name;
        private int maxMonitorWidth;
        private List<Monitor> monitors;

        public string Id { get { return id; } set { id = value;} }
        public string Name { get { return name; } set { name = value; } }
        public int MaxMonitorWidth { get { return maxMonitorWidth; } set { maxMonitorWidth = value; } }
        public List<Monitor> Monitors { get { return monitors; } set { monitors = value; } }
    
        public Agent()
        {
            maxMonitorWidth = 0;
            monitors = new List<Monitor>();
        }


        /// <summary>
        /// Add a monitor to the list and keep track of the maximum width
        /// of the display width of the monitor name
        /// </summary>
        /// <param name="_monitor"></param>
        public void AddMonitor(Monitor _monitor)
        {
            if (_monitor.Name.Length > maxMonitorWidth)
                maxMonitorWidth = _monitor.Name.Length;

            monitors.Add(_monitor);
        }


        /// <summary>
        /// Output the agent results to the screen
        /// </summary>
        public void DisplayAgent()
        {
            // write the agent title
            Console.WriteLine("\n----------------------------------------------------------------");
            Console.WriteLine(this.Name);
            Console.WriteLine("----------------------------------------------------------------");

            // write each monitor of this agent
            foreach (Monitor monitor in this.monitors)
                monitor.DisplayMonitor();

            Console.Write("\n");
        }


        /// <summary>
        /// Get the global monitors specified on the commandline
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="client"></param>
        /// <param name="showMonitors"></param>
        public void GetGlobalMonitors(string apiKey, HttpClient client, List<string> argMonitors)
        {
            using (HttpResponseMessage response = client.Get("api?action=agentInfo&apikey=" + apiKey + "&output=xml&agentId=" + this.Id + "&loadTests=true"))
            {
                response.EnsureStatusIsSuccessful();

                String data = response.Content.ReadAsString();
                XDocument xml = XDocument.Parse(data);

                // copy the list of monitors from the command line
                List<string> showMonitors = new List<string>(argMonitors);

                // check if the list of monitors to show is 'all' or 'empty'
                if (showMonitors.Count == 0)
                {
                    foreach (MonitorDefinition md in GlobalMonitor.GetMonitorDefinitions())
                        showMonitors.Add(md.Metric);
                }

                // Loop through the list of monitors to show and retrieve the monitor details
                foreach (string m in showMonitors)
                {
                    // select the monitor having the name that is requested
                    var allMonitors = from monitorNode in xml.Descendants(m)
                                      select new
                                      {
                                          Id = monitorNode.Element("id").Value,
                                          Name = Util.BaseMonitorName(monitorNode.Element("name").Value, '@')
                                      };

                    // create the monitor objects and copy in the values from the
                    // XML results
                    foreach (var a in allMonitors)
                    {
                        Monitor monitor = new GlobalMonitor(this);
                        monitor.Id = a.Id;
                        monitor.Name = a.Name;
                        monitor.GetMetrics(apiKey, client);
                        this.AddMonitor(monitor);
                    }
                }
            }
        }


        /// <summary>
        /// Retrieve the process monitors
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="client"></param>
        /// <param name="showMonitors"></param>
        public void GetProcessMonitors(string apiKey, HttpClient client, List<string> showProcesses)
        {
            using (HttpResponseMessage response = client.Get("api?action=agentInfo&apikey=" + apiKey + "&output=xml&agentId=" + this.Id + "&loadTests=true"))
            {
                response.EnsureStatusIsSuccessful();

                String data = response.Content.ReadAsString();
                XDocument xml = XDocument.Parse(data);

                // select all process monitors
                var allMonitors = from monitorNode in xml.Descendants("process")
                                    select new
                                    {
                                        Id = monitorNode.Element("id").Value,
                                        Name = monitorNode.Element("processName").Value
                                    };

                // filter the process monitors based on the name and create
                // the monitor objects and fill in the values
                foreach (var a in allMonitors)
                {
                    foreach (string m in showProcesses)
                    {
                        if (a.Name.ToLower().Contains(m.ToLower()) || m.ToLower() == "all")
                        {
                            Monitor monitor = new ProcessMonitor(this);
                            monitor.Id = a.Id;
                            monitor.Name = a.Name;
                            monitor.GetMetrics(apiKey, client);
                            this.AddMonitor(monitor);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Retrieve the process monitors
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="client"></param>
        /// <param name="showMonitors"></param>
        public void GetExternalMonitors(string apiKey, HttpClient client, List<string> argMonitors)
        {
            using (HttpResponseMessage response = client.Get("api?apikey=" + apiKey + "&output=xml&version=2&action=tests"))
            {
                response.EnsureStatusIsSuccessful();

                String data = response.Content.ReadAsString();
                XDocument xml = XDocument.Parse(data);

                // copy the list of monitors from the command line
                List<string> showMonitors = new List<string>(argMonitors);

                // check if the list of monitors to show is 'all' or 'empty'
                if (showMonitors.Count == 0)
                {
                    foreach (MonitorDefinition md in ExternalMonitor.GetMonitorDefinitions())
                        showMonitors.Add(md.Metric);
                }


                // filter the monitors from the returned XML
                var allMonitors = from monitorNode in xml.Descendants("test")
                                    select new
                                    {
                                        Id = monitorNode.Attribute("id").Value,
                                        Name = monitorNode.Value
                                    };

                // create all the monitor objects and copy in the values from the XML results
                foreach (var a in allMonitors)
                {
                    foreach (string m in showMonitors)
                    {
                        if (a.Name.ToLower().Contains(m.ToLower()) || m.ToLower() == "all")
                        {
                            Monitor monitor = new ExternalMonitor(this);
                            monitor.Id = a.Id;
                            monitor.Name = a.Name;
                            monitor.GetMetrics(apiKey, client);
                            this.AddMonitor(monitor);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Retrieve the fullpage monitors
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="client"></param>
        /// <param name="showMonitors"></param>
        public void GetFullpageMonitors(string apiKey, HttpClient client, List<string> argMonitors)
        {
            using (HttpResponseMessage response = client.Get("api?version=2&apikey=" + apiKey + "&output=xml&action=topFullpage&limit=50&detailedResults=true"))
            {
                response.EnsureStatusIsSuccessful();

                String data = response.Content.ReadAsString();
                XDocument xml = XDocument.Parse(data);

                // copy the list of monitors from the command line
                List<string> showMonitors = new List<string>(argMonitors);

                // check if the list of monitors to show is 'all' or 'empty'
                if (showMonitors.Count == 0)
                {
                    foreach (MonitorDefinition md in FullpageMonitor.GetMonitorDefinitions())
                        showMonitors.Add(md.Metric);
                }


                // filter the monitors from the returned XML
                var allMonitors = from monitorNode in xml.Descendants("test")
                                  select new
                                  {
                                      Id = monitorNode.Element("id").Value,
                                      Name = monitorNode.Element("testName").Value
                                  };

                // create all the monitor objects and copy in the values from the XML results
                foreach (var a in allMonitors)
                {
                    foreach (string m in showMonitors)
                    {
                        if (a.Name.ToLower().Contains(m.ToLower()) || m.ToLower() == "all")
                        {
                            Monitor monitor = new FullpageMonitor(this);
                            monitor.Id = a.Id;
                            monitor.Name = a.Name;
                            monitor.GetMetrics(apiKey, client);
                            this.AddMonitor(monitor);
                        }
                    }
                }
            }
        }
    }
}


