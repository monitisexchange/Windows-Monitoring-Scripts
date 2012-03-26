using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Monitis
{
    public class Authentication : APIObject
    {
        /// <summary>
        /// Actions of Authentication class
        /// </summary>
        private enum AuthenticationAction
        {
            apikey,
            secretkey,
            userkey,/*Does not exist in doc*/
            authToken
        }

        #region Authentication constructors and methods

        /// <summary>
        /// Constructor wihtout params. Don't forget to call Authenticate to fill apikey, secretkey and authToken
        /// </summary>
        public Authentication()
        {

        }

        /// <summary>
        /// Authentication user by his name and password
        /// </summary>
        /// <param name="userName">Name of user</param>
        /// <param name="password">Password of user</param>
        /// <param name="output">Type of output. If not selected, uses global params (JSON is default). Otherwise, set global param</param>
        public Authentication(string userName, string password, OutputType? output=null)
        {
            OutputGlobal = GetOutput(output);
            Authenticate(userName, password);
        }

        /// <summary>
        /// Authentication by apiKey, secretKey (optional) and authToken (optional)
        /// </summary>
        /// <param name="apiKey">ApiKey of user</param>
        /// <param name="secretKey">SecretKey. If null, it will be get by ApiKey</param>
        /// <param name="authToken">AuthToken. If null, it will be get by ApiKey and SecretKey</param>
        public Authentication(string apiKey, string secretKey = null, string authToken = null, OutputType? output=null)
        {
            OutputGlobal = GetOutput(output);
            this.apiKey = apiKey;
            this.secretKey = string.IsNullOrEmpty(secretKey) ? GetSecretKey() : secretKey;
            this.authToken = string.IsNullOrEmpty(authToken) ? GetAuthToken() : authToken;
        }

        /// <summary>
        /// Gets apiKey, secretKey and authToken
        /// </summary>
        /// <param name="userName">Name of user</param>
        /// <param name="password">Password of user</param>
        /// <param name="output">Set global output (or use exists - JSON by default)</param>
        public void Authenticate(string userName, string password,OutputType? output=null)
        {
            OutputGlobal = GetOutput(output);
            apiKey = GetApiKey(userName, password, OutputGlobal);
            secretKey = GetSecretKey();
            authToken = GetAuthToken();
        }

        #endregion

        /// <summary>
        /// Helper method to get MD5 hash from string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>MD5 hash</returns>
        private static string GetMD5Hash(string input)
        {
            var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            var s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }

        /// <summary>
        /// Using this action you can get apikey by providing userName and password. Apikey is mandatory both in POST and GET requsests. 
        /// </summary>
        /// <param name="userName">an e-mail of a registered user which acts as a username</param>
        /// <param name="password">	User's password. It will be encrypted with MD5 algorithm.</param>
        /// <param name="output">Output type - JSON or XML</param>
        /// <returns>Apikey</returns>
        public string GetApiKey(string userName, string password, OutputType? output=null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.userName, userName);
            string passwordMD5Hash = GetMD5Hash(password);
            parameters.Add(Params.password, passwordMD5Hash);
            RestResponse response = MakeGetRequest(AuthenticationAction.apikey, parameters, outputType);
            return Helper.GetValueOfKey(response, AuthenticationAction.apikey, outputType);
        }

        /// <summary>
        /// Using this action you can get secretkey by providing apikey. Secretkey is mandatory in POST requsests. 
        /// </summary>
        /// <param name="apikey">user's apikey</param>
        /// <param name="output">Output type - JSON or XML</param>
        /// <returns>secretKey</returns>
        public string GetSecretKey(string apikey = null, OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            AddIfNotNull(parameters,Params.apikey,apikey);
            RestResponse response = MakeGetRequest(AuthenticationAction.secretkey,parameters:parameters, output: outputType);
            return Helper.GetValueOfKey(response, AuthenticationAction.secretkey, outputType);
        }
        
        /// <summary>
        /// Using this action you can get authentication token, which is used in POST requests. 
        /// If apikey and secretkey is not specified, they will be got from base params (this.apiKey and this.secretKey)
        /// </summary>
        /// <param name="apikey">user's apikey</param>
        /// <param name="secretkey">user's secretkey</param>
        /// <param name="output">Output type - JSON or XML</param>
        /// <returns>AuthToken</returns>
        public string GetAuthToken(string apikey = null, string secretkey = null, OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            if (string.IsNullOrEmpty(apikey))
                apikey = this.apiKey;
            AddIfNotNull(parameters, Params.apikey, apikey);
            if (string.IsNullOrEmpty(secretkey))
                secretkey = this.secretKey;
            AddIfNotNull(parameters, Params.secretkey, secretkey);
            RestResponse response = MakeGetRequest(AuthenticationAction.authToken, parameters, outputType);
            return Helper.GetValueOfKey(response, AuthenticationAction.authToken, outputType);
        }
    }
}
