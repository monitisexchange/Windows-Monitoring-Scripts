using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Monitis.ProfilerConfigEditor
{
    public class ConfigFile
    {
        public ConfigFile(string path, BindingList<ConfigElementM> configElements)
        {
            Path = path;
            ConfigElements = configElements;
        }

        public string Path { get; set; }
        public BindingList<ConfigElementM> ConfigElements { get; set; }
    }
}
