using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monitis
{
    public abstract class URLObject
    {
        public const string DATA_SEPARATOR = ";";
        public const string KEY_VALUE_SEPARATOR = ":";

        public abstract String ToUrlString();

        public static String ToUrlString(IEnumerable<URLObject> list)
        {
            String str = "";
            foreach (URLObject obj in list)
            {
                str += obj.ToUrlString() + DATA_SEPARATOR;
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - DATA_SEPARATOR.Length);
            }
            return StringUtils.UrlEncode(str);
        }

        public static String MapToURLString(IDictionary map)
        {
            String str = "";
            foreach (object key in map.Keys)
            {
                str += StringUtils.UrlEncode(key.ToString()) + KEY_VALUE_SEPARATOR +
                       StringUtils.UrlEncode(map[key].ToString()) + DATA_SEPARATOR;
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - DATA_SEPARATOR.Length);
            }
            return StringUtils.UrlEncode(str);
        }
    }
}
