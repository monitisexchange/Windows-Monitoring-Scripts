using System;

namespace Monitis
{
    public class MonResult : URLObject
    {
        private String _paramName;
        private String _paramValue;

        public MonResult(String paramName, String paramValue)
        {
            _paramName = paramName;
            _paramValue = paramValue;
        }

        public MonResult(String urlString)
        {
            String[] fields = urlString.Split(new[] {KEY_VALUE_SEPARATOR}, StringSplitOptions.None);
            _paramName = StringUtils.UrlDecode(fields[0]);
            _paramValue = StringUtils.UrlDecode(fields[1]);
        }

        public override String ToUrlString()
        {
            return
                StringUtils.UrlEncode(_paramName) + KEY_VALUE_SEPARATOR +
                StringUtils.UrlEncode(_paramValue);
        }

        public String GetParamName()
        {
            return _paramName;
        }

        public void SetParamName(String paramName)
        {
            _paramName = paramName;
        }

        public String GetParamValue()
        {
            return _paramValue;
        }

        public void SetParamValue(String paramValue)
        {
            _paramValue = paramValue;
        }
    }
}