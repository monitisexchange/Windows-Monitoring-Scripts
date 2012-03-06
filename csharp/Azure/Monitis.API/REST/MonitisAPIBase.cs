using System;
using Monitis.API.Common;

namespace Monitis.API.REST
{
    /// <summary>
    /// Abstract class for all types and implementations API calls
    /// </summary>
    public abstract class MonitisAPIBase
    {
        #region constructors

        protected MonitisAPIBase(String apiKey, APIType apiType, String requestUrl)
        {
            Validation.EmptyOrNull(apiKey, "apiKey");
            Validation.EmptyOrNull(requestUrl, "requestUrl");

            APIKey = apiKey;
            APIType = apiType;
            RequestUrl = requestUrl;
        }

        #endregion constructors

        #region public properties

        public String APIKey { get; private set; }

        public APIType APIType { get; private set; }

        public String RequestUrl { get; private set; }

        #endregion public properties

        #region protected methods

        /// <summary>
        /// Return api client object for single request
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        protected APIClient GetApiClient(String actionName)
        {
            Validation.EmptyOrNull(actionName, "actionName");

            return new APIClient(APIType, APIKey, RequestUrl, actionName);
        }

        /// <summary>
        /// Return api client object for single request
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        protected APIClient GetApiClient(String actionName, String authToken)
        {
            Validation.EmptyOrNull(actionName, "actionName");
            Validation.EmptyOrNull(authToken, "authToken");
            return new APIClient(APIType, APIKey, RequestUrl, actionName, authToken);
        }



        #endregion protected methods
    }
}