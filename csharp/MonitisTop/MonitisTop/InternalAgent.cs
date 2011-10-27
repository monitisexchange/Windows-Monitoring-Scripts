using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Http;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace MonitisTop
{
    public class agent
    {
        public string Id;
        public string Key;
        public string Status;
        public string Platform;
    }


    public class agents 
    {
        public List<agent> agentList;
    }

    public class InternalAgent
    {
        private string apiKey = "AVUU86JKUKERMF4LUF15RAAPS";
        private HttpClient client;
       // private List<Agent> agents;



        public InternalAgent()
        {
            client = new HttpClient("http://www.monitis.com/");
            using (HttpResponseMessage response = client.Get("api?action=agents&apikey=" + apiKey + "&output=xml"))
            {
                response.EnsureStatusIsSuccessful();

                agents root = response.Content.ReadAsDataContract<agents>();
                
                foreach (agent ag in root.agentList)
                {
                    Console.WriteLine(ag.Id);
                    Console.WriteLine(ag.Key);
                }
                
            }

        }

    }
}
