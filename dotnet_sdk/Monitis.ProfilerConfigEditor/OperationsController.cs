using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Monitis.ProfilerConfigEditor
{
    public class OperationsController
    {
        private ConfigFileMXMLRepository _repository;

        public OperationsController()
        {
            _repository = new ConfigFileMXMLRepository(Config.ExstensionsPath);
        }

        public void ReloadConfigs()
        {
            try
            {
                _repository.LoadData();
                FilesChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }

        public void SaveConfigs()
        {
            try
            {
                _repository.CommitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }

        public void AddNewConfigFile()
        {
            try
            {
                FormNewConfig form = new FormNewConfig();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _repository.AddNew(form.ConfigFileName);
                    FilesChanged(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }

        public void AddNewConfigRec(ConfigFile file)
        {
            try
            {
                file.ConfigElements.Add(new ConfigElementM());
                FilesChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }

        public void SaveConfig(ConfigFile file)
        {
            try
            {
                _repository.CommitChanges(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }

        public void DeleteConfig(ConfigFile file)
        {
            try
            {
                _repository.RemoveFile(file);
                FilesChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }

        public event EventHandler FilesChanged = (sender, args) => { };

        public IEnumerable<ConfigFile> Files { get { return _repository.Files; } }
    }
}
