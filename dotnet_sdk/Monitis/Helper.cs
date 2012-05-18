using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Monitis
{
    /// <summary>
    /// Type of output - xml or json
    /// </summary>
    public enum OutputType { XML, JSON }

    /// <summary>
    /// Type of server validation requests - HMACSHA1 or token
    /// </summary>
    public enum Validation
    {
        HMACSHA1,
        token
    }

    /// <summary>
    /// Helper class
    /// TODO: change to internal
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// http://monitis.com by default
        /// </summary>
        public const string UrlServer = "http://monitis.com";
        /// <summary>
        /// http://monitis.com/customMonitorApi by default
        /// </summary>
        public const string UrlCustomMonitorApi = UrlServer + @"/customMonitorApi";
        /// <summary>
        /// http://monitis.com/api by default
        /// </summary>
       public const string UrlApi = UrlServer + @"/api";
        /// <summary>
        /// Current version = 2
        /// </summary>
        public const string ApiVersion = "2";
        /// <summary>
        /// ";" - separator
        /// </summary>
        public const string DataSeparator = ";";

        /// <summary>
        /// ":" - separator for key and value
        /// </summary>
        public const string KeyValueSeparator = ":";

        /// <summary>
        /// Converts bool to int (1 or 0)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int? BoolToInt(bool? b)
        {
            int? result=null;
            if (b.HasValue)
            {
                if (b == true)
                    result = 1;
                else if (b == false)
                {
                    result = 0;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets value of key from json or xml string
        /// If contains "error", throws exception
        /// </summary>
        /// <param name="content">Content of json or xml</param>
        /// <param name="key">Finds value of this key</param>
        /// <param name="outputType">XML of JSON</param>
        /// <returns>Value of key</returns>
        public static string GetValueOfKey(string content, string key, OutputType outputType)
        {
            string value = string.Empty;
            if (outputType == OutputType.JSON)
            {
                value = Json.GetValueOfKey(content, key);
            }
            else if (outputType == OutputType.XML)
            {
                value = Xml.GetValueOfKey(content, key);
            }
            return value;
        }

        public static string MapToURLString(Dictionary<string, string> map)
        {
            string str = string.Empty;
            foreach (KeyValuePair<string, string> keyValuePair in map)
            {
                str += keyValuePair.Key + KeyValueSeparator +keyValuePair.Value + DataSeparator;
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - DataSeparator.Length);
            }
            //return "[" + str + "]";
            return  str ;
        }

        public static T DeserializeObject<T>(string content, OutputType outputType, string xmlRoot=null)
        {
            T result = default(T);
            if (OutputType.JSON == outputType)
            {
                result = Json.DeserializeObject<T>(content);
            }
            else if (OutputType.XML == outputType)
            {
                result = Xml.DeserializeObject<T>(content,xmlRoot);
            }
            return result;
        }

        public static T DeserializeObject<T>(RestResponse response, OutputType outputType, string xmlRoot=null)
        {
            return DeserializeObject<T>(response.Content, outputType, xmlRoot);
        }

        /// <summary>
        /// If status is not "ok", throws exception
        /// </summary>
        /// <param name="response">response with status</param>
        /// <param name="output">output type</param>
        public static void CheckStatus(RestResponse response, OutputType output)
        {
            string status = GetValueOfKey(response, Params.status, output);
            if (status != Params.ok)
                throw new Exception(status);
        }

        /// <summary>
        /// Gets value of key from json or xml string
        /// If contains "error", throws exception
        /// </summary>
        /// <param name="response">Response of REST service</param>
        /// <param name="key">Finds value of this key</param>
        /// <param name="outputType">XML of JSON</param>
        /// <returns>Value of key</returns>
        public static string GetValueOfKey(RestResponse response, string key, OutputType outputType)
        {
            return GetValueOfKey(response.Content, key,outputType);
        }

        /// <summary>
        /// Gets value of key from json or xml string
        /// If contains "error", throws exception
        /// </summary>
        /// <param name="response">Response of REST service</param>
        /// <param name="key">Finds value of this key</param>
        /// <param name="outputType">XML of JSON</param>
        /// <returns>Value of key</returns>
        public static string GetValueOfKey(RestResponse response, Enum key, OutputType outputType)
        {
            return GetValueOfKey(response.Content, key.ToString(), outputType);
        }

        /// <summary>
        /// Helper for Json
        /// </summary>
        public static class Json
        {
            public static T DeserializeObject<T>(string content)
            {
                return JsonConvert.DeserializeObject<T>(content);
            }

            public static T DeserializeObject<T>(RestResponse response)
            {
                return DeserializeObject<T>(response.Content);
            }

            #region GetValueOfKey
            
            /// <summary>
            /// Gets value of specified key
            /// If contains "error", throws exception
            /// </summary>
            /// <param name="content"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public static string GetValueOfKey(string content, string key)
            {
                var error = JObject.Parse(content)[Params.error];
                if (null != error)
                    throw new Exception(error.ToString());

                //Get value without recursion
                JObject jObjectParsed = JObject.Parse(content);
                JToken valueParsed = jObjectParsed[key];
                if (null != valueParsed)
                    return valueParsed.ToString();
                //recursion
                else
                {
                    foreach (JToken jToken in jObjectParsed.Values())
                    {
                        string result = GetValueOfTokenKey(jToken, key);
                        if (null != result)
                            return result;
                    }
                }

                return null;
            }

            /// <summary>
            /// Get value of key recursively
            /// </summary>
            /// <param name="jToken"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            private static string GetValueOfTokenKey(JToken jToken, string key)
            {
                foreach (JToken jTokenCurrent in jToken.Values())
                {
                    if (((JProperty)jTokenCurrent.Parent).Name == key)
                        return jTokenCurrent.ToString();
                    else
                    {
                        string result = GetValueOfTokenKey(jTokenCurrent, key);
                        if (null != result)
                            return result;
                    }
                }
                return null;
            }

            public static string GetValueOfKey(RestResponse response, string key)
            {
                return GetValueOfKey(response.Content,key);
            }

            public static string GetValueOfKey(RestResponse response, Enum key)
            {
                return GetValueOfKey(response.Content, key.ToString()); ;
            }
            #endregion
        }

        /// <summary>
        /// Helper for Xml
        /// </summary>
        public static class Xml
        {
            public static T DeserializeObject<T>(string content, string xmlRoot)
            {
                XmlSerializer xmlSerializer;
                if (xmlRoot != null)
                    xmlSerializer = new XmlSerializer(typeof(T),new XmlRootAttribute(xmlRoot));
                else
                    xmlSerializer = new XmlSerializer(typeof (T));

                return (T) xmlSerializer.Deserialize(new StringReader(content));
            }

            public static T DeserializeObject<T>(RestResponse response, string xmlRoot)
            {
                return DeserializeObject<T>(response.Content, xmlRoot);
            }

            /// <summary>
            /// Gets value of specified key
            /// If contains "error", throws exception
            /// </summary>
            /// <param name="content"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public static string GetValueOfKey(string content, string key)
            {
                string result = string.Empty;
                var doc = XDocument.Parse(content);
                var error = (GetElementWithName(doc.Elements(), Params.error));
                if (null!=error)
                    throw new Exception(error.Value);
                result = GetElementWithName(doc.Elements(), key).Value;
                return result;
            }

            /// <summary>
            /// Search element with given name recursively 
            /// </summary>
            /// <param name="elements"></param>
            /// <param name="name"></param>
            /// <returns></returns>
            private static XElement GetElementWithName(IEnumerable<XElement> elements, string name)
            {
                //Search in parent node
                foreach (var element in elements)
                {
                    if (element.Name.LocalName == name)
                        return element;
                }
                //Search in child nodes
                foreach (var element in elements)
                {
                    var result = GetElementWithName(element.Elements(), name);
                    if (result != null)
                        return result;
                }
                return null;
            }

            /// <summary>
            /// Gets value of specified key
            /// If contains "error", throws exception
            /// </summary>
            /// <param name="response"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public static string GetValueOfKey(RestResponse response, string key)
            {
                return GetValueOfKey(response.Content, key);
            }

            /// <summary>
            /// Gets value of specified key
            /// If contains "error", throws exception
            /// </summary>
            /// <param name="response"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public static string GetValueOfKey(RestResponse response, Enum key)
            {
                return GetValueOfKey(response.Content, key.ToString()); ;
            }
        }
    }

    
}
