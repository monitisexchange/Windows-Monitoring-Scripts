using System;
using Monitis.API.Common;
using Monitis.API.REST.User.Contract;

namespace Monitis.API.REST.User
{
    /// <summary>
    /// Contains API methods for work with user
    /// </summary>
    public class UserAPI : MonitisAPIBase
    {
        public UserAPI(String apiKey, APIType apiType)
            : base(apiKey, apiType, APIResources.UserAPIPath)
        {
        }

        /// <summary>
        /// Return secretkey for apikey
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns>Secret key value</returns>
        public String GetSecretKey()
        {
            APIClient apiClient = GetApiClient(ActionNames.SecretKey);
            SecretKeyResponce result = apiClient.InvokeGet<SecretKeyResponce>();

            return result.SecretKey;
        }

        /// <summary>
        /// Get auth token value
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public String GetAuthToken(String secretKey)
        {
            Validation.EmptyOrNull(secretKey, "secretKey");

            APIClient apiClient = GetApiClient(ActionNames.AuthToken);
            apiClient.AddParam(ParamNames.SecretKey, secretKey);

            var response = apiClient.InvokeGet<AuthTokenResponse>();
            return response.AuthToken;
        }
    }
}