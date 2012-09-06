using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Monitis.ProfilerConfigEditor
{
    public class ConfigFileMXMLRepository
    {
        public IEnumerable<ConfigFile> Files { get; set; }
        private readonly string _repositoryPath;
        private List<ConfigFile> FilesList { get { return Files as List<ConfigFile>; } }

        public ConfigFileMXMLRepository(string repositoryPath)
        {
            this._repositoryPath = repositoryPath;
            Files = new List<ConfigFile>();
        }

        public void LoadData()
        {
            FilesList.Clear();
            if (!Directory.Exists(_repositoryPath))
                return;

            foreach (var path in XmlFilesPaths())
            {
                FilesList.Add(new ConfigFile(path, new BindingList<ConfigElementM>(new List<ConfigElementM>(GetConfigElements(path)))));
            }
        }

        public void RemoveFile(ConfigFile file)
        {
            if (File.Exists(file.Path))
                File.Delete(file.Path);
            FilesList.Remove(file);
        }

        public ConfigFile AddNew(string fileName)
        {
            var file = new ConfigFile(Path.Combine(_repositoryPath,fileName), new BindingList<ConfigElementM>());
            FilesList.Add(file);
            return file;
        }

        public void CommitChanges()
        {
            foreach (var configFile in FilesList)
            {
                Serialize(configFile.Path, configFile.ConfigElements);
            }
        }

        public void CommitChanges(ConfigFile configFile)
        {
            Serialize(configFile.Path, configFile.ConfigElements);
        }

        private IEnumerable<string> XmlFilesPaths()
        {
            string[] xmlFilesPaths = null;
            string path = _repositoryPath;
            if (null != path)
            {
                xmlFilesPaths = Directory.GetFiles(path, "*.xml");
            }
            return xmlFilesPaths;
        }

        private IEnumerable<ConfigElementM> GetConfigElements(string path)
        {
            var xmlDocument = XDocument.Load(path);

            XElement documentElement = xmlDocument.Root;
            if (null != documentElement)
            {
                var instrumentationNode = documentElement.Descendants().FirstOrDefault();
                foreach (XElement unitProfile in instrumentationNode.Descendants())
                {
                    foreach (var configElement in ConfigElementFactoryM.CreateNodes(unitProfile))
                    {
                        yield return configElement;
                    }
                }
            }
        }

        private void Serialize(string path, IEnumerable<ConfigElementM> elements)
        {
            var xmlDocument = new XDocument();

            var rootNode = new XElement("extensionMonitisProfiler");
            var configurationNode = new XElement("configuration");

            foreach (var nameGroup in elements.GroupBy(u => u.Name))
            {
                var unitProfileNode = new XElement("unitProfile");
                unitProfileNode.SetAttributeValue("name", nameGroup.Key ?? "");
                foreach (var coincidence1 in nameGroup.GroupBy(u => u.AssemblyName))
                {
                    foreach (var coincidence2 in coincidence1.GroupBy(u => u.ClassName))
                    {
                        var coincidenceNode = new XElement("coincidence");
                        coincidenceNode.SetAttributeValue("assemblyName", coincidence1.Key ?? "");
                        coincidenceNode.SetAttributeValue("className", coincidence2.Key ?? "");
                        foreach (var methodCondition in coincidence2)
                        {
                            var methodConditionNode = new XElement("methodCondition");
                            methodConditionNode.SetAttributeValue("methodName", methodCondition.MethodName??"");
                            methodConditionNode.SetAttributeValue("parameters", methodCondition.Parameters ?? "");
                            coincidenceNode.Add(methodConditionNode);
                        }
                        unitProfileNode.Add(coincidenceNode);
                    }
                }
                configurationNode.Add(unitProfileNode);
            }
            rootNode.Add(configurationNode);
            xmlDocument.Add(rootNode);
            xmlDocument.Save(path);
        }
    }
}
