using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RestSharp;

namespace Monitis
{
    public class APIObject
    {
        public string apiUrl = Helper.UrlApi;
        public string apiKey = "";
        public string secretKey = "";
        public string authToken = "";

        #region Helper methods

        /// <summary>
        /// Set apiKey, secretKey and authToken
        /// </summary>
        /// <param name="authentication"></param>
        public void SetAuthenticationParams(Authentication authentication)
        {
            apiKey = authentication.apiKey;
            secretKey = authentication.secretKey;
            authToken = authentication.authToken;
        }

        /// <summary>
        /// Add key value pair to dictonary, if value is not null
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void AddIfNotNull(Dictionary<string, object> dictionary, string key, object value)
        {
            if (value!=null)
            {
                dictionary.Add(key,value);
            }
        }

        internal OutputType GetOutput(OutputType? currentOutput)
        {
            if (currentOutput.HasValue)
                return currentOutput.Value;
            else
                return OutputGlobal;
        }

        internal Validation GetValidation(Validation? validation)
        {
            if (validation.HasValue)
                return validation.Value;
            else
                return ValidationGlobal;
        }

        private static string CalculateRFC2104HMAC(string paramValueString, string secretKey)
        {
            string result = string.Empty;
            var myhmacsha1 = new HMACSHA1();
            myhmacsha1.Key = Encoding.ASCII.GetBytes(secretKey);
            byte[] sigBaseStrByteArr = Encoding.UTF8.GetBytes(paramValueString);
            byte[] hashValue = myhmacsha1.ComputeHash(sigBaseStrByteArr);
            result = Convert.ToBase64String(hashValue);
            return result;
        }

        #endregion

        /// <summary>
        /// Global outputType
        /// Default value is JSON 
        /// </summary>
        public OutputType OutputGlobal = OutputType.JSON;

        /// <summary>
        /// Global validation type for POST requests 
        /// Default value is HMACSHA1
        /// </summary>
        public Validation ValidationGlobal = Validation.HMACSHA1;

        /// <summary>
        /// Default apiUrl is http://monitis.com/api
        /// </summary>
        public APIObject()
        {
            apiUrl = Helper.UrlApi;
        }

        public APIObject(Authentication authentication)
        {
            SetAuthenticationParams(authentication);
        }

        /// <summary>
        /// Constructor with specified apiKey and secretKey.  Default apiUrl is http://monitis.com/api
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        public APIObject(string apiKey, string secretKey)
        {
            this.apiKey = apiKey;
            this.secretKey = secretKey;
            apiUrl = Helper.UrlApi;
        }

        /// <summary>
        /// Constructor with specified apiKey, secretKey and apiUrl
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        /// <param name="apiUrl"></param>
        public APIObject(string apiKey, string secretKey, string apiUrl)
        {
            this.apiKey = apiKey;
            this.secretKey = secretKey;
            this.apiUrl = apiUrl;
        }

        /// <summary>
        /// Post request with specified params
        /// </summary>
        /// <param name="action">Action for post request</param>
        /// <param name="output">Type of output from server</param>
        /// <param name="parameters">Params for post request</param>
        /// <param name="validation">Type of request's validation on server</param>
        /// <returns>Response from server</returns>
        internal RestResponse MakePostRequest(Enum action,
            Dictionary<string, object> parameters, OutputType? output = null, Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            var requestParams = new Dictionary<string, object>();
            requestParams.Add(Params.output, outputType);

            Validation validationCurrent = GetValidation(validation);
            requestParams.Add(Params.validation, validationCurrent);

            DateTime curTime = DateTime.UtcNow;
            string formattedTime = curTime.ToString("yyyy-MM-dd HH:mm:ss");
            requestParams.Add(Params.action, action);
            requestParams.Add(Params.apikey, apiKey);
            requestParams.Add(Params.timestamp, formattedTime);
            requestParams.Add(Params.version, Helper.ApiVersion);


            if (parameters != null)
                PutAll(requestParams, parameters);

            //Order for put request
            requestParams = requestParams.OrderBy(r => r.Key).ToDictionary(r => r.Key, r => r.Value);

            var paramValueString = new StringBuilder();
            foreach (var reqParam in requestParams)
            {
                paramValueString.Append(reqParam.Key);
                paramValueString.Append(reqParam.Value);
            }

            if (Validation.HMACSHA1 == validationCurrent)
            {
                //secretkey for checksum
                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new Exceptions.SecretKeyIsNullOrEmptyException();
                }
                string checkSum = CalculateRFC2104HMAC(paramValueString.ToString(), secretKey);
                requestParams.Add(Params.checksum, checkSum);
            }
            else if (Validation.token == validationCurrent)
            {
                if (string.IsNullOrEmpty(authToken))
                {
                    throw new Exceptions.AuthTokenisNullOrEmptyException();
                }
                requestParams.Add(Params.authToken, authToken);
            }

            var restClient = new RestClient(apiUrl);
            var restRequest = new RestRequest(Method.POST);
            foreach (var reqParam in requestParams)
            {
                restRequest.AddParameter(reqParam.Key, reqParam.Value);
            }

            RestResponse response = restClient.Execute(restRequest);
            return response;
        }

        /// <summary>
        /// Get request with specified params
        /// </summary>
        /// <param name="action">Action for get request</param>
        /// <param name="parameters">Params for get request</param>
        /// <param name="output">Type of request's validation on server</param>
        /// <returns>Response from server</returns>
        internal RestResponse MakeGetRequest(Enum action,
            Dictionary<string, object> parameters = null, OutputType? output=null)
        {
            OutputType outputType = GetOutput(output);
            var requestParams = new Dictionary<string, object>();
            requestParams.Add(Params.output, outputType);

            requestParams.Add(Params.action, action);
            requestParams.Add(Params.apikey, apiKey);
            requestParams.Add(Params.version, Helper.ApiVersion);
            
            if (parameters != null)
                PutAll(requestParams, parameters);

            var restClient = new RestClient(apiUrl);
            var restRequest = new RestRequest(Method.GET);
            foreach (var reqParam in requestParams)
            {
                restRequest.AddParameter(reqParam.Key, reqParam.Value);
            }

            RestResponse response = restClient.Execute(restRequest);
            return response;
        }

        /// <summary>
        /// Replace all items in dictionary1 with items in dictionary2 and adds items dictionary2 to dictionary1
        /// </summary>
        /// <param name="dictionary1"></param>
        /// <param name="dictionary2"></param>
        private static void PutAll(Dictionary<string, object> dictionary1, Dictionary<string, object> dictionary2)
        {
            foreach (KeyValuePair<string, object> keyValuePair in dictionary2)
            {
                if (dictionary1.ContainsKey(keyValuePair.Key))
                {
                    dictionary1.Remove(keyValuePair.Key);
                }
                dictionary1.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }
    }
}
