using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Monitis.ProfilerConfigEditor
{
    public class ConfigElement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">"tracerFactory" if element exist and "instrumentation" - if new</param>
        /// <param name="newElement"></param>
        public ConfigElement(XmlNode node, bool newElement=false)
        {
            if (newElement)
            {
                CreateEmptyElement(node);
            }
            else
            {
                _tracerFactory = node;
                _match = node.FirstChild;
                _exactMethodMatcher = _match.FirstChild;
            }
        }

        private void CreateEmptyElement(XmlNode node)
        {
            XmlDocument document = node.OwnerDocument;
            XmlElement tracerFactory = document.CreateElement("tracerFactory");
            XmlAttribute name = document.CreateAttribute("name");
            tracerFactory.Attributes.Append(name);

            XmlElement match = document.CreateElement("match");
            XmlAttribute assemblyName = document.CreateAttribute("assemblyName");
            match.Attributes.Append(assemblyName);
            XmlAttribute className = document.CreateAttribute("className");
            match.Attributes.Append(className);
            tracerFactory.AppendChild(match);

            XmlElement exactMethodMatcher = document.CreateElement("exactMethodMatcher");
            XmlAttribute methodName = document.CreateAttribute("methodName");
            exactMethodMatcher.Attributes.Append(methodName);
            XmlAttribute parameters = document.CreateAttribute("parameters");
            exactMethodMatcher.Attributes.Append(parameters);
            match.AppendChild(exactMethodMatcher);

            node.AppendChild(tracerFactory);

            _tracerFactory = tracerFactory;
            _match = match;
            _exactMethodMatcher = exactMethodMatcher;
        }

        public void Delete()
        {
            _tracerFactory.ParentNode.RemoveChild(_tracerFactory);
        }

        private XmlNode _tracerFactory=null;

        private XmlNode _match=null;

        private XmlNode _exactMethodMatcher=null;

        public string Name
        {
            get { return _tracerFactory.Attributes["name"].Value; }
            set { _tracerFactory.Attributes["name"].Value = value; }
        }

        public string AssemblyName
        {
            get { return _match.Attributes["assemblyName"].Value; }
            set { _match.Attributes["assemblyName"].Value = value; }
        }

        public string ClassName
        {
            get { return _match.Attributes["className"].Value; }
            set { _match.Attributes["className"].Value = value; }
        }

        public string MethodName
        {
            get { return _exactMethodMatcher.Attributes["methodName"].Value; }
            set { _exactMethodMatcher.Attributes["methodName"].Value = value; }
        }

        public string Parameters
        {
            get { return _exactMethodMatcher.Attributes["parameters"].Value; }
            set { _exactMethodMatcher.Attributes["parameters"].Value = value; }
        }
    }
}
