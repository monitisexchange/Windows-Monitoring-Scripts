using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Http;
using System.Xml.Linq;

namespace MonitisTop
{
    public class AgentList : List<Agent>
    {
        // Default constructor
        public AgentList() {}


        /// <summary>
        /// Retrieve the agents
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="client"></param>
        public void GetAgents(string apiKey, HttpClient client)
        {
            using (HttpResponseMessage response = client.Get("api?action=agents&apikey=" + apiKey + "&output=xml"))
            {
                response.EnsureStatusIsSuccessful();

                String data = response.Content.ReadAsString();
                XDocument xml = XDocument.Parse(data);

                var allAgents = from agentNode in xml.Descendants("agent")
                                select new
                                {
                                    Id = agentNode.Element("id").Value,
                                    Key = agentNode.Element("key").Value
                                };


                foreach (var a in allAgents)
                {
                    Agent agent = new Agent();
                    agent.Id = a.Id;
                    agent.Name = a.Key;
                    this.Add(agent);
                }
            }
        }


        /// <summary>
        /// Output the results of all the agents in the list
        /// </summary>
        public void DisplayAgents()
        {
            foreach (Agent agent in this)
                agent.DisplayAgent();
        }
    }
}
