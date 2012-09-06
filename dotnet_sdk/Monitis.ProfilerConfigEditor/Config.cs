using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace Monitis.ProfilerConfigEditor
{
    public class Config
    {
        public static string ExstensionsPath
        {
             get
            {
                string value = null;
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Monitis");
                if (null != registryKey)
                {
                    value = registryKey.GetValue("ExtensionsPath").ToString();
                }
                return value;
            }
        }
    }
}
