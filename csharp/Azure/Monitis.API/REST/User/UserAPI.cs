using System;
using Monitis.API.Common;
using Monitis.API.REST.User.Contract;

namespace Monitis.API.REST.User
{
    /// <summary>
    /// Contains API methods for work with user
    /// </summary>
    public class UserAPI
    {
        public const String RequestUrl = "/api";

        /// <summary>
        /// Return secretkey for apikey
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns>Secret key value</returns>
        public String GetSecretKey(String apiKey)
        {
            Validation.ValidateAPIKey(apiKey);

            APIClient apiClient = new APIClient(APIType.Live, apiKey, RequestUrl, ActionNames.SecretKey);
            SecretKeyResponce result = apiClient.InvokeGet<SecretKeyResponce>();
            return result.SecretKey;
        }

        /// <summary>
        /// Get auth token value
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public String GetAuthToken(String apiKey, String secretKey)
        {
            Validation.ValidateAPIKey(apiKey);

            Validation.ValidateSecretKey(secretKey);

            var apiClient = new APIClient(APIType.Live, apiKey, RequestUrl, ActionNames.AuthToken);
            apiClient.AddParam(ParamNames.SecretKey, secretKey);
            var response = apiClient.InvokeGet<AuthTokenResponse>();
            return response.AuthToken;
        }
    }
}