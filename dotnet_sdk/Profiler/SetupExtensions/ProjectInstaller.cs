using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;


namespace SetupExtensions
{
    [RunInstaller(true), ComVisible(false)]
    public partial class ProjectInstaller : Installer
    {
        private string _registryInstalledPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Monitis";

        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {

            try
            {
                var fullpath = Context.Parameters["P_TargetDir"].ToString(CultureInfo.InvariantCulture);
                var path = fullpath.Remove(fullpath.Length - 2);

                Registry.SetValue(_registryInstalledPath, "InstalledPath", path);
                    /*.LocalMachine.OpenSubKey(_registryInstalledPath, true);

                if (softBase == null)
                {
                    softBase = Registry.LocalMachine.CreateSubKey(_registryInstalledPath);
                }
                softBase.SetValue("InstalledPath", path);
                softBase.Flush();*/
                base.Install(stateSaver);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
