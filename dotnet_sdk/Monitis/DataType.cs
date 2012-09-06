using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monitis
{
    public enum DataType
    {
        Boolean = 1,
        Integer = 2,
        String = 3,
        Float = 4
    }

    public static class DataTypeHelper
    {
        public static Object GetValue(DataType dataType, Object value)
        {
            if (value == null) return null;
            String str = value.ToString();
            switch (dataType)
            {
                case DataType.Boolean:
                    return bool.Parse(str);
                case DataType.Integer:
                    return int.Parse(str);
                case DataType.String:
                    return str;
                case DataType.Float:
                    return float.Parse(str);
            }
            return value;
        }

        public static DataType ValueOf(int id)
        {
            return (DataType) id;
        }
    }
}