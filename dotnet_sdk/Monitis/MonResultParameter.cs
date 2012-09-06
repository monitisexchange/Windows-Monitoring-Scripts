using System;
using System.Collections.Generic;
using System.Linq;

namespace Monitis
{
    public class MonResultParameter : URLObject
    {
        private readonly DataType dataType;
        private String _displayName;
        private String _name;
        private String _uom;

        /**
         * 
         * @param name
         * @param displayName
         * @param uom
         * @param dataType
         */

        public MonResultParameter(String name, String displayName, String uom, DataType dataType)
        {
            _name = name;
            _displayName = displayName;
            _uom = uom;
            this.dataType = dataType;
        }

        /**
         * 
         * @param urlString
         * @throws MonitisException
         */

        public MonResultParameter(String urlString)
        {
            String[] fields = urlString.Split(new[] {KEY_VALUE_SEPARATOR}, StringSplitOptions.None);
            _name = StringUtils.UrlDecode(fields[0]);
            _displayName = StringUtils.UrlDecode(fields[1]);
            _uom = StringUtils.UrlDecode(fields[2]);
            dataType = DataTypeHelper.ValueOf(int.Parse(fields[3]));
        }

        public override String ToUrlString()
        {
            return StringUtils.UrlEncode(_name) + KEY_VALUE_SEPARATOR +
                   StringUtils.UrlEncode(_displayName) + KEY_VALUE_SEPARATOR +
                   StringUtils.UrlEncode(_uom) + KEY_VALUE_SEPARATOR +
                   (int) dataType;
        }

        public static List<MonResultParameter> GetObjectList(String str)
        {
            string[] urlStringParts = str.Split(new[] {DATA_SEPARATOR}, StringSplitOptions.None);
            List<MonResultParameter> parameters =
                urlStringParts.Select(urlStr => new MonResultParameter(urlStr)).ToList();
            return parameters;
        }

        public String GetDisplayName()
        {
            return _displayName;
        }

        public void SetDisplayName(String displayName)
        {
            _displayName = displayName;
        }

        public String getName()
        {
            return _name;
        }

        public void setName(String name)
        {
            _name = name;
        }

        public String getUom()
        {
            return _uom;
        }

        public void setUom(String uom)
        {
            _uom = uom;
        }
    }
}