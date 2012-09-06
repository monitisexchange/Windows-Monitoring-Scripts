using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp.Contrib;

namespace Monitis
{
    public static class StringUtils
    {
        public static String GetFormatedString(String str, int length)
        {
            var returnStr = str.Substring(0, str.Length < length ? str.Length : length);
            returnStr += GetString(" ", length - returnStr.Length);
            return returnStr;
        }

        public static String GetString(String substr, int count)
        {
            var str = "";
            for (int i = 0; i < count; i++)
            {
                str += substr;
            }
            return str;
        }

        public static String ArrayToString(Object[] array, String separator, int beginIndex, int endIndex)
        {
            var buf = new StringBuilder();
            for (int i = beginIndex; i < array.Length && i < endIndex; buf.Append(array[i++]).Append(separator)) ;
            String encoded = buf.ToString();
            return encoded;
        }

        public static String UrlEncode(String str)
        {
            String encodedStr;
            try
            {

                encodedStr = HttpUtility.UrlEncode(str, Encoding.UTF8); ;
            }
            catch (Exception e)
            {
                throw new Exceptions.MonitisException("Error encoding string: " + str, e);
            }
            return encodedStr;
        }

        public static String UrlDecode(String str)
        {
            String decodedStr;
            try
            {
                decodedStr = HttpUtility.UrlDecode(str, Encoding.UTF8);
            }
            catch (Exception e)
            {
                throw new Exceptions.MonitisException("Error decoding string: " + str, e);
            }
            return decodedStr;
        }

        /*static readonly sbyte[] HEX_CHAR_TABLE = {
    	(sbyte)'0', (sbyte)'1', (sbyte)'2', (sbyte)'3',
    	(sbyte)'4', (sbyte)'5', (sbyte)'6', (sbyte)'7',
    	(sbyte)'8', (sbyte)'9', (sbyte)'a', (sbyte)'b',
    	(sbyte)'c', (sbyte)'d', (sbyte)'e', (sbyte)'f'
	};

        public unsafe static String getHexString(byte[] raw)
        {

            sbyte[] hex = new sbyte[2 * raw.Length];
            fixed (sbyte* ptr = hex)
            {

                int index = 0;

                foreach (byte b in raw)
                {
                    int v = b & 0xFF;
                    hex[index++] = HEX_CHAR_TABLE[v >> 4];
                    hex[index++] = HEX_CHAR_TABLE[v & 0xF];
                }

                return new String(ptr, 0, hex.Length, Encoding.ASCII);
            }
        }

        public static sbyte[] hexStringToByteArray(String s)
        {
            int len = s.length();
            byte[] data = new byte[len / 2];
            for (int i = 0; i < len; i += 2)
            {
                data[i / 2] = (byte)((Character.digit(s.charAt(i), 16) << 4)
                                     + Character.digit(s.charAt(i + 1), 16));
            }
            return data;
        }


        public static String join(Collection s, String delimiter)
        {
            StringBuffer buffer = new StringBuffer();
            Iterator iter = s.iterator();
            if (iter.hasNext())
            {
                buffer.append(iter.next());
                while (iter.hasNext())
                {
                    buffer.append(delimiter);
                    buffer.append(iter.next());
                }
            }
            return buffer.toString();
        }

        public static String join(Object[] ar, String delimiter)
        {
            StringBuffer buffer = new StringBuffer();
            for (Object obj :
            ar)
            {
                buffer.append(obj);
                buffer.append(delimiter);
            }
            if (ar.length > 0) buffer.deleteCharAt(buffer.length() - 1);
            return buffer.toString();
        }*/

        public static string Join(int[] agentIds, string s)
        {
            return String.Join(s, agentIds);
        }
    }

}
