using System;
using System.Windows.Forms;
using Monitis.Prototype.Logic.Azure;
using Monitis.Prototype.Logic.Azure.TableService;
using Monitis.Prototype.Logic.Common;

namespace Monitis.Prototype.UI
{
    public partial class AzureTableServiceLogin : Form
    {
        private readonly UserSession _userSession;

        public AzureTableServiceLogin(UserSession userSession, Form mdiParent)
        {
            MdiParent = mdiParent;
            _userSession = userSession;
            InitializeComponent();
        }

        private void OnApplySettingsClick(object sender, EventArgs e)
        {
            Util.ShowWaitWindow("Try connect...");
            try
            {
                Boolean isCanConnect = TableServiceManager.TryConnect(tbxStorageAccountName.Text.Trim(), tbxAccountPrivateKey.Text.Trim());
                if (isCanConnect)
                {
                    Util.UpdateWaitWindowDisplay("Check table exists...");
                    Boolean isPerfomanceTableExists = TableServiceManager.IsPerfomanceTableExists(tbxStorageAccountName.Text.Trim(),
                                                                                     tbxAccountPrivateKey.Text.Trim());
                    if(isPerfomanceTableExists)
                    {
                        //now we can show mediator and start process
                        _userSession.AzureInfo = new AzureInfo
                                                     {
                                                         AccountKey = tbxAccountPrivateKey.Text.Trim(),
                                                         AccountName = tbxStorageAccountName.Text.Trim(),
                                                         DeploymentInfo = new DeploymentInfo
                                                                              {
                                                                                  DeploymentID =
                                                                                      tbxDeploymentID.Text.Trim()
                                                                              }
                                                     };
                        MediatorForm mediatorForm = new MediatorForm(_userSession, MdiParent);
                        mediatorForm.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Look like no Perfomance table in you storage account.");
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
            }
            finally
            {
                Util.CloseWaitWindow();
            }
        }

        private void OnUseDefaultCheckedChanged(object sender, EventArgs e)
        {
            if (cbUseDefault.Checked)
            {
                LoadDefaultStorageSettings();
                ChangeInputLock(true);
            }
            else
            {
                CleanStorageSettingsInput();
                ChangeInputLock(false);
            }
        }

        /// <summary>
        /// Change input lock
        /// </summary>
        private void ChangeInputLock(Boolean isLocked)
        {
            tbxStorageAccountName.Enabled = tbxAccountPrivateKey.Enabled = tbxDeploymentID.Enabled = !isLocked;
        }

        /// <summary>
        /// Set to empty all Azure table and deployment settings
        /// </summary>
        private void CleanStorageSettingsInput()
        {
            tbxAccountPrivateKey.Text = tbxAccountPrivateKey.Text = tbxDeploymentID.Text = String.Empty;
        }

        /// <summary>
        /// Load default storage account info and deployment info to form
        /// </summary>
        private void LoadDefaultStorageSettings()
        {
            tbxStorageAccountName.Text = TableServiceManager.DefaultAccountName;
            tbxAccountPrivateKey.Text = TableServiceManager.DefaultAccountKey;
            tbxDeploymentID.Text = DeploymentInfo.DefaultDeploymentID;
        }
    }
}