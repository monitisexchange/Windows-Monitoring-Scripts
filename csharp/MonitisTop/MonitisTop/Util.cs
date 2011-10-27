using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitisTop
{
    public static class Util
    {
        /// <summary>
        /// Pad a given string with blank spaces
        /// </summary>
        /// <param name="inString"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Pad(string input, int len)
        {
            string tmp = input;
            while (tmp.Length < len)
                tmp += ' ';

            return tmp;
        }


        /// <summary>
        /// Returns the base monitor name.
        /// For example: 'drive_C' will return 'drive'
        /// </summary>
        /// <param name="input"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string BaseMonitorName(string input, char delimiter)
        {
            int i = input.IndexOf(delimiter);
            string baseMonitorName = input;
            if (i > 0)
                baseMonitorName = input.Substring(0, i).ToLower();

            return baseMonitorName;
        }
    }
}
