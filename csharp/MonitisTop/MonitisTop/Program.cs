using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Http;
using System.Xml.Linq;


namespace MonitisTop
{
    class Program
    {
        // API access
        private static string apiKey = GetAPIKey();
        private static HttpClient client = new HttpClient("http://www.monitis.com/");

        // specify which agent monitor types we can support
        private static string[] SupportedAgentTypes = { "int", "ext", "full" };

        // specify if we need to show just monitors or process monitors
        private static bool useMonitors = false;
        private static bool useProcesses = false;

        // command line parameters
        private static string argCommand = "";
        private static string argApiKey = "";
        private static string argSecretKey = "";
        private static List<string> argAgentTypes = new List<string>();
        private static List<string> argProcesses = new List<string>();
        private static List<string> argMonitors = new List<string>();

        private static Dictionary<string, string> options = new Dictionary<string, string>();
        private static Dictionary<string, Agent> agents = new Dictionary<string, Agent>();
        private static Dictionary<string, Monitor> monitors = new Dictionary<string, Monitor>();


        static void Main(string[] args)
        {
            if (ParseCommands(args))
                ProcessCommands();
        }

        
        /// <summary>
        /// Collect command line parameters and split them into named arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool ParseCommands(string[] args)
        {
            // collect command line parameters and split them into named arguments
            foreach (string arg in args)
            {
                string[] pieces = arg.Split(':');
                options[pieces[0]] = pieces.Length > 1 ? pieces[1] : "";
            }

            // parse command line options
            if (options.Count > 0)
            {
                // check for help
                if (options.ContainsKey("/help"))
                {
                    showUsage("Available Options:");
                    return false;
                }

                // get the command to execute
                if (options.ContainsKey("/cmd"))
                    argCommand = options["/cmd"];
                else
                {
                    showUsage("Invalid or missing command");
                    return false;
                }


                switch (argCommand)
                {
                    case "listagents":
                            // get the agent monitor types to query
                            if (options.ContainsKey("/type"))
                            {
                                // are we show all agents?
                                if (options["/type"].ToLower() == "all")
                                    argAgentTypes.AddRange(SupportedAgentTypes);
                                else
                                    argAgentTypes.AddRange(options["/type"].Split(',').ToList());

                                // validate the agent types entered
                                foreach (string s in argAgentTypes)
                                {
                                    if (!SupportedAgentTypes.Contains(s.ToLower()))
                                    {
                                        showUsage("Invalid monitor type speficied");
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                showUsage("Invalid or missing monitor type");
                                return false;
                            }

                            // get the monitors to query
                            if (options.ContainsKey("/monitors"))
                            {
                                useMonitors = true;

                                if (options["/monitors"] == "")
                                {
                                    showUsage("Please specify the names of the monitors to query");
                                    return false;
                                }
                                else if (options["/monitors"].ToLower() == "all")
                                {
                                    // Do nothing here. We'll pass an empty list of showMonitors
                                    // which will return the results for ALL monitors
                                    ;
                                }
                                else
                                {
                                    // Add the list of entered monitors to the list of monitors to show
                                    argMonitors.AddRange(options["/monitors"].Split(',').ToList());
                                }
                            }


                            // get the processes to query
                            if (options.ContainsKey("/processes"))
                            {
                                useProcesses = true;
                                argProcesses.AddRange(options["/processes"].Split(',').ToList());

                                if (argProcesses.Count == 0)
                                {
                                    showUsage("Please specify the names of the proceses to query");
                                    return false;
                                }
                            }

                            // make sure there are monitors or processes provided on the commandline
                            if (argCommand == "listagents" && (!useMonitors && !useProcesses))
                            {
                                showUsage("please specify monitors or processes to query");
                                return false;
                            }
                            break;

                    case "setkeys":
                            // get the keys from the commandline
                            if (options.ContainsKey("/apikey"))
                                argApiKey = options["/apikey"];
                            else
                            {
                                showUsage("Please provide the APIKey value");
                                return false;
                            }

                            if (options.ContainsKey("/secretkey"))
                                argSecretKey = options["/secretkey"];
                            else
                            {
                                showUsage("Please provide the SecretKey value");
                                return false;
                            }

                            break;
                }


                // all command line options check out
                return true;
            }

            // no command line options provided at all
            showUsage("Missing commandline option");
            return false;
        }


        /// <summary>
        /// Process the commands
        /// </summary>
        private static void ProcessCommands()
        {

            // process the command
            switch (argCommand)
            {
                case "listagents":
                    // create the Agent list
                    AgentList agentList = new AgentList();

                    if (argAgentTypes.Contains("int"))
                        ListInternalAgents(agentList);

                    if (argAgentTypes.Contains("ext"))
                        ListExternalAgents(agentList);

                    if (argAgentTypes.Contains("full"))
                        ListFullpageAgents(agentList);

                    // Display the results
                    agentList.DisplayAgents();
                    break;

                case "setkeys":
                    SetKeys(argApiKey, argSecretKey);
                    break;

                case "getkeys":
                    Console.WriteLine("APIKey: {0}", GetAPIKey());
                    Console.WriteLine("SecretKey: {0}", GetSecretKey());
                    break;
            }
        }


        /// <summary>
        /// List agents and collect monitor information
        /// </summary>
        private static void ListInternalAgents(AgentList agentList)
        {
            // Retrieve all the agents defined
            agentList.GetAgents(apiKey, client);

            // Process each Agent
            foreach (Agent agent in agentList)
            {
                if (useMonitors)
                    agent.GetGlobalMonitors(apiKey, client, argMonitors);

                if (useProcesses)
                    agent.GetProcessMonitors(apiKey, client, argProcesses);
            }
        }


        /// <summary>
        /// List the external agent monitors
        /// </summary>
        private static void ListExternalAgents(AgentList agentList)
        {
            // Add a dummy agent
            Agent dummy = new Agent();
            dummy.Id = "9999";
            dummy.Name = "EXTERNAL";
            agentList.Add(dummy);

            dummy.GetExternalMonitors(apiKey, client, argMonitors);
        }


        /// <summary>
        /// List the fullpage monitors
        /// </summary>
        private static void ListFullpageAgents(AgentList agentList)
        {
            // Add a dummy agent
            Agent dummy = new Agent();
            dummy.Id = "9999";
            dummy.Name = "FULLPAGE";
            agentList.Add(dummy);

            dummy.GetFullpageMonitors(apiKey, client, argMonitors);
        }


        /// <summary>
        /// Retrieve the API key from config.xml
        /// </summary>
        /// <returns></returns>
        private static string GetAPIKey()
        {
            string key = "";

            try
            {
                XElement root = XElement.Load("config.xml");
                key = (string)(from el in root.Descendants("APIKey")
                               select el).First();
                return key;
            }
            catch
            {
                showUsage("CONFIG.XML not found");
            }

            return key;
        }



        /// <summary>
        /// Retrieve the Secret key from config.xml
        /// </summary>
        /// <returns></returns>
        private static string GetSecretKey()
        {
            string key = "";

            try
            {
                XElement root = XElement.Load("config.xml");
                key = (string)(from el in root.Descendants("SecretKey")
                               select el).First();
                return key;
            }
            catch
            {
                showUsage("CONFIG.XML not found");
            }

            return key;
        }



        /// <summary>
        /// Set the ASI and Secretkey
        /// </summary>
        /// <returns></returns>
        private static void SetKeys(string apiKey, string secretKey)
        {
            FileInfo fi = new FileInfo("config.xml");

            String s = "<monitis><APIKey>" + apiKey + "</APIKey><SecretKey>" + secretKey + "</SecretKey></monitis>";

            TextWriter writer = new StreamWriter("config.xml");
            writer.WriteLine(s);
            writer.Close();
        }


        /// <summary>
        /// Show Usage
        /// </summary>
        /// <param name="strError"></param>
        private static void showUsage(string strInfo)
        {
            Console.WriteLine("Error: {0}\n", strInfo);
            Console.WriteLine("See usage examples below\n");
            Console.WriteLine("Show command line help:");
            Console.WriteLine("/help\n");
            Console.WriteLine("View APIKey and SecretKey values:");
            Console.WriteLine("/cmd:getkeys\n");
            Console.WriteLine("Set APIKey and SecretKey:");
            Console.WriteLine("/cmd:setkeys /apikey:<apikey> /secretkey:<secretKey>\n");
            Console.WriteLine("Show global monitor results for defined agents:");
            Console.WriteLine("/cmd:listagents /type:<int>|<ext>|<fullpage>|<all> [/monitors:<all><name>,<name>,...]\n");
            Console.WriteLine("Show monitor results for one or more specific processes:");
            Console.WriteLine("/cmd:listagents /type:<int>|<ext>|<all> [/process:<name>,<name>,...]\n");
            Console.WriteLine("Example:");
            Console.WriteLine("Show cpu and memory monitors for all internal agents");
            Console.WriteLine("/cmd:listagents /type:int /monitors:cpu,memory\n");
        }
    }
}
