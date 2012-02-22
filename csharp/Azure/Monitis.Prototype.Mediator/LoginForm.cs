using System.Text;
using System.Windows.Forms;
using Monitis.Prototype.Logic.Common;

namespace Monitis.Prototype.UI
{
    /// <summary>
    /// Represents class for Login form to monitis.com
    /// </summary>
    public partial class LoginForm : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();
            if(tbxMonitisAPIKey.Text.Length > 0)
            {
                btnLogin.Enabled = true;
            }
        }

        /// <summary>
        /// Handler for api key textbox text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMonitisAPIKeyTextChanged(object sender, System.EventArgs e)
        {
            if (tbxMonitisAPIKey.Text.Trim().Length > 0)
            {
                btnLogin.Enabled = true;
            }
            else
            {
                btnLogin.Enabled = false;
            }
        }

        /// <summary>
        /// Handler for button login click 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoginClick(object sender, System.EventArgs e)
        {
            Util.ShowWaitWindow();

            //create and start new user session to Monitis
            var userSession = new UserSession(tbxMonitisAPIKey.Text.Trim());
            ActionResult actionResult = userSession.Start();

            Util.CloseWaitWindow();
            if (actionResult.IsSuccessful)
            {
                MonitorsForm monitorsForm = new MonitorsForm(userSession, MdiParent);
                Close();
                monitorsForm.Show();
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var error in actionResult.Errors)
                {
                    stringBuilder.AppendLine(error);
                }
                MessageBox.Show(stringBuilder.ToString(), "Error");
            }
        }
    }
}