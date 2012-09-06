using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AgentCore;
using Monitis;
using log4net;

namespace ConsoleTestAgent
{
    class Program
    {
        public static string Login = "monitis_test@adoriasoft.com";
        public static string Password = "SuperPassword!";
        public static string ApiKey = "3HQB2PHSMHCRRHV5M9FR8MTBC";
        public static string SekretKey = "1H4AMF6FEO72IEVH8FD5B4US61";
        public static string FirstName = "monitis";
        public static string LastName = "test";
        private static ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                Log.Info("Starting");
                Environment.SetEnvironmentVariable("COR_ENABLE_PROFILING", "1", EnvironmentVariableTarget.User);
                Environment.SetEnvironmentVariable("COR_PROFILER", "{71EDB19D-4F69-4A2C-A2F5-BE783F543A7E}",
                                                   EnvironmentVariableTarget.User);
                var a = new Authentication(Login, Password, null);
                var analizer = new MonitisClrMethodInfoAnalizer(a);
                var agent = new Agent(analizer, new ClrActionsProcessor());
                System.Console.ReadKey();
                agent.Stop();
                Environment.SetEnvironmentVariable("COR_ENABLE_PROFILING", "0", EnvironmentVariableTarget.User);
                Log.Info("Ending");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
