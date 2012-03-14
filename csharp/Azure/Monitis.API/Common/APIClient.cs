using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Microsoft.Http;

namespace Monitis.API.Common
{
    /// <summary>
    /// Represents client for Monitis REST API.
    /// <remarks>Current implementation support authentication over token.
    /// Each time when need invoke request to API create new <see cref="APIClient"/> object. Not reuse object between two different requests.
    /// Not thread safe.
    /// </remarks>
    ///
    /// </summary>
    public class APIClient
    {
        #region static members

        /// <summary>
        /// URL to Monitis Sandbox API
        /// </summary>
        public static String SandboxURL = APIResources.SandboxAPIHostUrl;

        /// <summary>
        /// URL to Monitis Live API
        /// </summary>
        public static String LiveURL = APIResources.LiveAPIHostUrl;

        /// <summary>
        /// Format for timestamp <see cref="ParamNames.Timestamp"/> parameter value
        /// </summary>
        public static String TimestampFormat = APIResources.TimestampFormatAPI;

        #endregion static members

        #region constructors

        /// <summary>
        /// API client constructor
        /// </summary>
        /// <param name="apiType">Sandbox or live</param>
        /// <param name="apiKey">API key</param>
        /// <param name="requestUrl">Url of destination API</param>
        /// <param name="action">Action of API which need to call</param>
        /// <param name="authToken">Auth token for security request to API</param>
        public APIClient(APIType apiType, String apiKey, String requestUrl, String action, String authToken)
            : this(apiType, apiKey, requestUrl, action)
        {
            _authToken = authToken;
        }

        /// <summary>
        ///  API client constructor
        /// </summary>
        /// <param name="apiType">Sandbox or live</param>
        /// <param name="apiKey">API key</param>
        /// <param name="requestUrl">Request API URL</param>
        /// <param name="action">Action of API which need to call</param>
        public APIClient(APIType apiType, String apiKey, String requestUrl, String action)
        {
            _requestUrl = requestUrl;
            _apiHost = (apiType == APIType.Live ? LiveURL : SandboxURL);
            _paramMap.Add(ParamNames.Action, action);
            _paramMap.Add(ParamNames.Apikey, apiKey);
        }

        #endregion constructors

        #region public methods

        /// <summary>
        /// Add parameter value to future request
        /// </summary>
        /// <param name="keyName">Key name</param>
        /// <param name="value">Key value</param>
        public void AddParam(String keyName, String value)
        {
            _paramMap[keyName] = value;
        }

        /// <summary>
        /// Invoke "GET" HTTP request to API
        /// </summary>
        /// <returns>Return result as string  in UTF8 format</returns>
        public String InvokeGet()
        {
            String joinParameters = JoinParameters();
            String queryString = String.Format("{0}?{1}", _requestUrl, joinParameters);
            String result;

            try
            {
                using (HttpClient httpClient = new HttpClient(_apiHost))
                {
                    HttpResponseMessage responseMessage = httpClient.Get(queryString);

                    Byte[] data = responseMessage.Content.ReadAsByteArray();
                    result = Encoding.UTF8.GetString(data);
                }
            }
            catch (Exception)
            {
                //TODO: log
                throw;
            }
            return result;
        }

        /// <summary>
        /// Invoke "POST" HTTP request to API
        /// </summary>
        /// <returns>Return resulst as string in UTF8 format</returns>
        public String InvokePost()
        {
            AddPostRequiredParams();
            String parameters = JoinParameters();

            //create http content for POST request
            HttpContent httpContent = HttpContent.Create(parameters, "application/x-www-form-urlencoded");
            String result;

            try
            {
                using (HttpClient httpClient = new HttpClient(_apiHost))
                {
                    HttpResponseMessage httpResponseMessage = httpClient.Post(_requestUrl, httpContent);

                    Byte[] data = httpResponseMessage.Content.ReadAsByteArray();
                    result = Encoding.UTF8.GetString(data);
                }
            }
            catch (Exception)
            {
                //TODO: log
                throw;
            }

            return result;
        }

        /// <summary>
        /// Invoke "POST" request
        /// </summary>
        /// <typeparam name="TResponse">Type of API response object</typeparam>
        /// <returns>Typed response object</returns>
        public TResponse InvokePost<TResponse>()
        {
            //get response as JSON string
            String jsonString = InvokePost();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TResponse deserializedObject = serializer.Deserialize<TResponse>(jsonString);
            return deserializedObject;
        }

        /// <summary>
        /// Invoke "GET" HTTP request to API
        /// </summary>
        /// <typeparam name="TResponse">Type of API response object</typeparam>
        /// <returns>Typed response object</returns>
        public TResponse InvokeGet<TResponse>()
        {
            //get response as JSON string
            String jsonString = InvokeGet();

            return new JavaScriptSerializer().Deserialize<TResponse>(jsonString);
        }

        #endregion public methods

        #region private methods

        /// <summary>
        /// Join parameters into request string with parameter encoding
        /// </summary>
        /// <returns></returns>
        private String JoinParameters()
        {
            List<String> paramKeyValue = new List<String>(_paramMap.Count);
            foreach (var p in _paramMap)
            {
                String s = String.Format("{0}={1}", p.Key, HttpUtility.UrlEncode(p.Value));
                paramKeyValue.Add(s);
            }
            return String.Join("&", paramKeyValue);
        }

        /// <summary>
        /// Add required parameters for future "POST" request
        /// </summary>
        private void AddPostRequiredParams()
        {
            AddParam(ParamNames.Timestamp, DateTime.Now.ToString(TimestampFormat));
            AddParam(ParamNames.Validation, ParamNames.Token);
            AddParam(ParamNames.AuthToken, _authToken);
        }

        #endregion private methods

        #region private fields

        private readonly Dictionary<String, String> _paramMap = new Dictionary<String, String>();
        private readonly String _authToken;
        private readonly String _requestUrl;
        private readonly String _apiHost;

        #endregion private fields
    }
}