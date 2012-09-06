using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Monitis.ProfilerConfigEditor
{
    public class ConfigElementM
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">"tracerFactory" if element exist and "instrumentation" - if new</param>
        /// <param name="newElement"></param>
        public ConfigElementM()
        {
        }

        public string Name { get; set; }

        public string AssemblyName { get; set; }

        public string ClassName { get; set; }

        public string MethodName { get; set; }

        public string Parameters { get; set; }
    }
}
