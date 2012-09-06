using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Globalization;
using System.IO;
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
                var path =Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Monitis"),"Extensions");
               
                Registry.SetValue(_registryInstalledPath, "ExtensionsPath", path);
                
                base.Install(stateSaver);
            }
            catch
            {
            }
        }
    }
}
