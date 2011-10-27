using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitisTop
{
    public class Metric
    {
        protected Monitor monitor;

        private string name;
        private string result;
        private string suffix;
        private int width;

        public string Name { get { return name; } set { name = value; } }
        public string Result { get { return result; } set { result = value; } }
        public string Suffix { get { return suffix; } set { suffix = value; } }
        public int Width { get { return width; } set { width = value; } }

        public Metric(Monitor _monitor)
        {
            monitor = _monitor;
        }

        public Metric(string _name, Monitor _monitor) : this(_monitor)
        {
            name = _name;
        }


        /// <summary>
        /// Output the metric column titles
        /// </summary>
        /// <param name="maxWidth"></param>
        public void DisplayMetricHeader()
        {
            Console.Write("{0}", Util.Pad(this.Name, monitor.MaxMetricWidth + 4));
        }


        /// <summary>
        /// Output the metric column values
        /// </summary>
        public void DisplayMetricResults()
        {
            Console.Write("{0}", Util.Pad(this.Result + this.Suffix, monitor.MaxMetricWidth + 4));
        }
    }
}
