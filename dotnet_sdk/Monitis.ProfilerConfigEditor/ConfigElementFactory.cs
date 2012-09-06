using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Monitis.ProfilerConfigEditor
{
    public static class ConfigElementFactoryM
    {
        public static IEnumerable<ConfigElementM> CreateNodes(XElement unitProfile)
        {

            return from XElement coincidence in unitProfile.Descendants()
                   from XElement methodCondition in coincidence.Descendants()
                   select new ConfigElementM()
                              {
                                  AssemblyName = coincidence.Attribute("assemblyName").Value,
                                  ClassName = coincidence.Attribute("className").Value,
                                  MethodName = methodCondition.Attribute("methodName").Value,
                                  Parameters = methodCondition.Attribute("parameters").Value,
                                  Name = unitProfile.Attribute("name").Value
                              };
        }
    }
}