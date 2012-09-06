using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Monitis;

namespace ProfilerLauncher
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        
        public string Login
        {
            get { return LogintextBox.Text; }
        }

        public string Password
        {
            get { return PasswordtextBox.Text; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoginToMonitis();
        }

        private void LoginToMonitis()
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
                DialogResult = DialogResult.Yes;
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
