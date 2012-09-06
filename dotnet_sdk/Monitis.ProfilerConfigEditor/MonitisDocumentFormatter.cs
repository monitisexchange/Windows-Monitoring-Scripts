using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Monitis.ProfilerConfigEditor
{
    public static class MonitisDocumentFormatter
    {
        public static void ReformatDocument(XmlDocument document)
        {
            var root = document.DocumentElement.FirstChild;
            IEnumerable<XmlNode> unitsProfile = GroupNodesInList(root, new List<string>() { "name" });

            RemoveChilds(root);
            foreach (XmlNode unitProfile in unitsProfile)
            {
                ReformatUnitProfile(unitProfile);
                root.AppendChild(unitProfile);
            }
        }

        private static void ReformatUnitProfile(XmlNode unitProfile)
        {
            IEnumerable<XmlNode> coincidences = GroupNodesInList(unitProfile, new List<string>() {"assemblyName", "className"});

            RemoveChilds(unitProfile);
            foreach (XmlNode coincidence in coincidences)
            {
                // ReformatCoincidence(coincidence);
                unitProfile.AppendChild(coincidence);
            }
        }

        private static void ReformatCoincidence(XmlNode coincidence)
        {
            IEnumerable<XmlNode> methodConditions = GroupNodesInList(coincidence, new List<string>() { "methodName", "parameters" });

            RemoveChilds(coincidence);
            foreach (XmlNode methodCondition in methodConditions)
            {
                coincidence.AppendChild(methodCondition);
            }
        }

        private static void RemoveChilds(XmlNode node)
        {
            List<XmlNode> nodesToDelete = new List<XmlNode>();
            foreach (XmlNode child in node.ChildNodes)
            {
                nodesToDelete.Add(child);
            }

            foreach (var xmlNode in nodesToDelete)
            {
                node.RemoveChild(xmlNode);
            }
        }

        private static IEnumerable<XmlNode> GroupNodesInList(XmlNode root, IEnumerable<string> attributes)
        {
            var groups = new List<XmlNode>();
            foreach (XmlNode child in root.ChildNodes)
            {
                var node = groups.FirstOrDefault(u => attributes.All(v => u.Attributes[v].Value== child.Attributes[(string) v].Value));
                if (node == null)
                {
                    groups.Add(child);
                }
                else
                {
                    foreach (XmlNode subChild in child.ChildNodes)
                    {
                        node.AppendChild(subChild);
                    }
                }
            }

            return groups;
        }
    }
}