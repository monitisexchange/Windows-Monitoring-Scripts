using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using AgentCore;
using Monitis;

namespace DotNetMonitisAgent
{
    public partial class DotNetMonitisAgent : ServiceBase
    {
        private Agent _agent;
        public static string Login;
        public static string Password;

        public DotNetMonitisAgent()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (args.Length < 2)
                return;
            Login = args[0];
            Password = args[1];

            var file = new StreamWriter(new FileStream("monitisagent.log",
     System.IO.FileMode.Append));
            try
            {
                file.Write("Starting service");
                var a = new Authentication(Login, Password, null);
                var mon = new MonitisClrMethodInfoAnalizer(a);
                var processor = new ClrActionsProcessor();
                _agent = new Agent(mon, processor);
                file.Write("Started service");
            }
            catch (Exception ex)
            {

                file.Write(ex.ToString());

                throw;
            }
            finally
            {
                file.Close();
            }

        }

        protected override void OnStop()
        {
            using (var file = new StreamWriter(new FileStream("monitisagent.log", System.IO.FileMode.Append)))
            {
                file.Write("Stopped service");
            }
            _agent.Dispose();
        }
    }
}
