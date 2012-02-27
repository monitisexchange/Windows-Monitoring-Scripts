using System;
using System.Linq;
using System.Windows.Forms;
using Monitis.API.Domain.Monitors;
using Monitis.API.REST.CustomMonitors;
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
            LoadMonitorList();
        }

        /// <summary>
        /// Load monitor list into drop down list
        /// </summary>
        private void LoadMonitorList()
        {
            Util.ShowWaitWindow("Loading monitors...");
            ddlMonitorList.Items.Clear();
            ddlMonitorList.ValueMember = "ID";
            ddlMonitorList.DisplayMember = "Name";
            foreach (var customMonitor in _userSession.GetCustomMonitors())
            {
                ddlMonitorList.Items.Add(customMonitor);
            }
            Util.CloseWaitWindow();
        }

        private void OnAddAzureMonitorClick(object sender, EventArgs e)
        {
            Util.ShowWaitWindow("Try create monitor...");
            _userSession.CreateAzureMonitors();
            LoadMonitorList();
            Util.CloseWaitWindow();
        }

        private void OnApplyMonitorClick(object sender, EventArgs e)
        {
            if (ddlMonitorList.SelectedItem != null)
            {
                Monitor monitor = ddlMonitorList.SelectedItem as Monitor;
                _userSession.CustomActiveMonitor = monitor;

                AzureTableServiceLogin azureTableServiceLogin = new AzureTableServiceLogin(_userSession, MdiParent);
                Close();
                azureTableServiceLogin.Show();
            }
        }
    }
}