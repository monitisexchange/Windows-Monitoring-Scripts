using System;
using System.Collections.Generic;
using System.Linq;

namespace Monitis
{
    public class MonitorParameter : URLObject
    {
        private DataType _dataType = DataType.String;
        private String _displayName = "";
        private bool _isHidden;
        private String _name;
        private String _value;

        /**
         * 
         * @param name
         * @param displayName
         * @param value
         * @param dataType
         * @param isHidden
         */

        public MonitorParameter(String name, String displayName, String value,
                                DataType dataType, bool isHidden)
        {
            _name = name;
            _displayName = displayName;
            _value = value;
            _dataType = dataType;
            _isHidden = isHidden;
        }

        /**
         * 
         * @param name
         * @param value
         */

        public MonitorParameter(String name, String value)
        {
            _name = name;
            _value = value;
        }

        public MonitorParameter(String urlString)
        {
            String[] fields = urlString.Split(new[] {KEY_VALUE_SEPARATOR}, StringSplitOptions.None);
            _name = StringUtils.UrlDecode(fields[0]);
            _displayName = StringUtils.UrlDecode(fields[1]);
            _value = StringUtils.UrlDecode(fields[2]);
            _dataType = DataTypeHelper.ValueOf(int.Parse(fields[3]));
            _isHidden = Boolean.Parse(fields[4]);
        }

        public override String ToUrlString()
        {
            return StringUtils.UrlEncode(_name) + KEY_VALUE_SEPARATOR +
                   StringUtils.UrlEncode(_displayName) + KEY_VALUE_SEPARATOR +
                   StringUtils.UrlEncode(_value) + KEY_VALUE_SEPARATOR +
                   (int) _dataType + KEY_VALUE_SEPARATOR +
                   _isHidden;
        }

        public static List<MonitorParameter> GetObjectList(String str)
        {
            String[] urlStringParts = str.Split(new[] {DATA_SEPARATOR}, StringSplitOptions.None);
            List<MonitorParameter> parameters = urlStringParts.Select(u => new MonitorParameter(u)).ToList();
            return parameters;
        }


        /**
         * @return the displayName
         */

        public String GetDisplayName()
        {
            return _displayName;
        }

        /**
         * @param displayName the displayName to set
         */

        public void SetDisplayName(String displayName)
        {
            _displayName = displayName;
        }

        /**
         * @return the name
         */

        public String GetName()
        {
            return _name;
        }

        /**
         * @param name the name to set
         */

        public void SetName(String name)
        {
            _name = name;
        }

        /**
         * @return the value
         */

        public String GetValue()
        {
            return _value;
        }

        /**
         * @param value the value to set
         */

        public void SetValue(String value)
        {
            _value = value;
        }

        /**
         * @return the dataType
         */

        public DataType GetDataType()
        {
            return _dataType;
        }

        /**
         * @param dataType the dataType to set
         */

        public void SetDataType(DataType dataType)
        {
            _dataType = dataType;
        }

        /**
         * @return the isHidden
         */

        public bool IsHidden()
        {
            return _isHidden;
        }

        /**
         * @param isHidden the isHidden to set
         */

        public void SetHidden(bool isHidden)
        {
            _isHidden = isHidden;
        }
    }
}