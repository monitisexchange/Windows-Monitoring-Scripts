using System;
using System.Windows.Forms;

namespace Monitis.Prototype.UI
{
    /// <summary>
    /// Represent form as MDI parent for all windows in appilcation
    /// </summary>
    public partial class HostFrom : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HostFrom()
        {
            InitializeComponent();
            LoginForm loginForm = new LoginForm { MdiParent = this };
            loginForm.Show();
        }

        /// <summary>
        /// Handler on Login Tool Strip Menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoginToolStripMenuItemClick(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm { MdiParent = this };
            loginForm.Show();
        }
    }
}
