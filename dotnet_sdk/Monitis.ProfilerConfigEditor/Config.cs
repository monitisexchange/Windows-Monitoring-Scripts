using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace Monitis.ProfilerConfigEditor
{
    public class Config
    {
        public static List<Tuple<XmlDocument, string>> Documents = new List<Tuple<XmlDocument, string>>();

        public static string NewConfigXmlFilePath { get; set; }

        public static void SaveAll()
        {
            foreach (var xmlDocument in Documents)
            {
                xmlDocument.Item1.Save(xmlDocument.Item2);
            }
        }

        public class ConfigList : BindingList<ConfigElement>
        {
            public XmlDocument XmlDocument { get; private set; }
            public string PathToSave { get; private set; }
            public XmlNode Instrumentation { get; private set; }

            public ConfigList(XmlNode instrumentation, XmlDocument xmlDocument, string pathToSave)
            {
                Instrumentation = instrumentation;
                XmlDocument = xmlDocument;
                PathToSave = pathToSave;
            }

            public void Save()
            {
                XmlDocument.Save(PathToSave);
            }

            protected override void RemoveItem(int index)
            {
                var deletingItem= this.Items[index];
                deletingItem.Delete();
                base.RemoveItem(index);
            }
        }

        public static string InstalledPath
        {
            get
            {
                string value = null;
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Monitis");
                if (null != registryKey)
                {
                    value = registryKey.GetValue("InstalledPath").ToString();
                }
                return value;
            }
        }

        public static string[] GetConfigsPaths()
        {
            string[] xmlFilesPaths = null;
            var configsAndPaths=new List<Tuple<string, string>>();
            string path = InstalledPath;
            if (null != path)
            {
                xmlFilesPaths = Directory.GetFiles(path, "*.xml");
            }
            return xmlFilesPaths;
        }

        public static ConfigList GetConfigElementsForFile(string filePath)
        {
            ConfigList configList = null;
            try
            {

                var xmlDocument = new XmlDocument();
                xmlDocument.Load(filePath);

                XmlElement documentElement = xmlDocument.DocumentElement;
                if (null != documentElement)
                {
                    //Instrumentation node
                    XmlNode instrumentationNode = documentElement.FirstChild;
                    configList = new ConfigList(instrumentationNode, xmlDocument, filePath);
                    //tracerFactory's
                    foreach (XmlNode tracerFactory in instrumentationNode.ChildNodes)
                    {
                        var configElement = new ConfigElement(tracerFactory);
                        configList.Add(configElement);
                    }
                }
                Documents.Add(new Tuple<XmlDocument, string>(xmlDocument, filePath));
            }
            catch (Exception ex)
            {
                
            }
            return configList;
        }
    }
}
