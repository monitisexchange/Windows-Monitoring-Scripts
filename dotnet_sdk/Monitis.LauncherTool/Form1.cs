using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace Monitis.LauncherTool
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            InitializeComponent();
            Load += new EventHandler(Launcher_Load);
        }

        void Launcher_Load(object sender, EventArgs e)
        {
            try
            {
                ShowCurrentState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }

        private void ShowCurrentState()
        {
            button1.Text = GetCurrentState() ? "Disable" : "Enable";
        }

        private bool GetCurrentState()
        {
            if (Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING", EnvironmentVariableTarget.User) == "1" &&
                Environment.GetEnvironmentVariable("COR_PROFILER", EnvironmentVariableTarget.User) != null &&
                Environment.GetEnvironmentVariable("COR_PROFILER", EnvironmentVariableTarget.User).ToLower() == "{71EDB19D-4F69-4A2C-A2F5-BE783F543A7E}".ToLower() &&
                DoesServiceExist("Monitis Agent") &&
                ((new ServiceController("Monitis Agent")).Status == ServiceControllerStatus.Running))
            {

                return true;

            }
            return false;
        }

        bool DoesServiceExist(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            var service = services.FirstOrDefault(s => s.ServiceName == serviceName);
            return service != null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetCurrentState())
                    Disable();
                else
                    Enable();
                ShowCurrentState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }

        private void Enable()
        {
            if (String.IsNullOrEmpty(LogintextBox.Text))
            {
                MessageBox.Show("Monitis Account Login is empty.");
                return;
            }
            if (String.IsNullOrEmpty(PasswordtextBox.Text))
            {
                MessageBox.Show("Monitis Account Password is empty.");
                return;
            } 
            if (!DoesServiceExist("Monitis Agent"))
            {
                MessageBox.Show("Monitis agent is not installed.");
                return;
            }
            var service = new ServiceController("Monitis Agent");
            try
            {
                if (service.Status != ServiceControllerStatus.Running)
                    service.Start(new string[]{LogintextBox.Text, PasswordtextBox.Text});
                service.WaitForStatus(ServiceControllerStatus.Running);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Monitis agent cannot start.");
            }
            Environment.SetEnvironmentVariable("COR_ENABLE_PROFILING", "1", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("COR_PROFILER", "{71EDB19D-4F69-4A2C-A2F5-BE783F543A7E}",
                                               EnvironmentVariableTarget.User);
        }

        private void Disable()
        {
            Environment.SetEnvironmentVariable("COR_ENABLE_PROFILING", "0", EnvironmentVariableTarget.User);

            var service = new ServiceController("Monitis Agent");
            try
            {
                if (service.Status != ServiceControllerStatus.Stopped)
                    service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Monitis agent cannot stop.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TestConnectionAndAccount();
        }

        private void TestConnectionAndAccount()
        {
            try
            {
                if (String.IsNullOrEmpty(LogintextBox.Text))
                {
                    MessageBox.Show("Monitis Account Login is empty.");
                    return;
                }
                if (String.IsNullOrEmpty(PasswordtextBox.Text))
                {
                    MessageBox.Show("Monitis Account Password is empty.");
                    return;
                }

                var auth = new Authentication();
                auth.Authenticate(LogintextBox.Text, PasswordtextBox.Text, OutputType.XML);
                MessageBox.Show("Success!");
            }
            catch (Exception ex)
            {
                if (ex.Message == "Invalid username or password")
                {
                    MessageBox.Show("Invalid username or password");
                }
                else
                {
                    MessageBox.Show("Can not connect to Monitis Server");
                }
            }
        }
    }
}
