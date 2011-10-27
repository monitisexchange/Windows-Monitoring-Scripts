using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitisTop
{
    public class MonitorDefinition
    {
        private string metric;
        private List<string> names;
        private List<string> suffixes;

        public string Metric { get { return metric; } set { metric = value; } }
        public List<string> Names { get { return names; } set { names = value; } }
        public List<string> Suffixes { get { return suffixes; } set { suffixes = value; } }

        public MonitorDefinition(string _metric)
        {
            metric = _metric;

            names = new List<string>();
            suffixes = new List<string>();
        }

        public void AddNamePair(string _name, string _suffix)
        {
            names.Add(_name);
            suffixes.Add(_suffix);
        }
    }
}
