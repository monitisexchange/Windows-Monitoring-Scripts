using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Monitis.Prototype.Logic.Common;

namespace Monitis.Prototype.UI
{
    public partial class MonitorsForm : Form
    {
        private readonly UserSession _userSession;

        public MonitorsForm(UserSession userSession, Form mdiParent)
        {
            if (userSession == null)
            {
                throw new ArgumentNullException("userSession");
            }

            MdiParent = mdiParent;
            _userSession = userSession;
            InitializeComponent();
            CheckMonitorList();
        }

        /// <summary>
        /// Load monitor list into drop down list
        /// </summary>
        private void CheckMonitorList()
        {
            btnCreateMonitors.Enabled = btnNext.Enabled = false;

            Util.ShowWaitWindow("Check monitors...");
            if (_userSession.CheckCustomMonitors())
            {
                lblInfo.Text = "All required monitors exists in monitis API. Hit Next to proceed";
                btnNext.Enabled = true;
            }
            else
            {
                lblInfo.Text = "Not all required monitors exists in monitis API. Hit Create Monitors to proceed";
                btnCreateMonitors.Enabled = true;
            }

            Util.CloseWaitWindow();
        }

        private void OnAddAzureMonitorClick(object sender, EventArgs e)
        {
            Util.ShowWaitWindow("Try create monitors...");
            List<String> failedMonitors = _userSession.CreateAzureMonitors();
            if (failedMonitors.Count > 0)
            {
                String message = failedMonitors.Aggregate(String.Empty, (current, failedMonitor) => current + String.Format("{0}\r\n", failedMonitor));
                MessageBox.Show(message, @"Failed to add monitors");
            }

            CheckMonitorList();
            Util.CloseWaitWindow();
        }

        private void OnApplyMonitorClick(object sender, EventArgs e)
        {
            AzureTableServiceLogin azureTableServiceLogin = new AzureTableServiceLogin(_userSession, MdiParent);
            Close();
            azureTableServiceLogin.Show();
        }
    }
}