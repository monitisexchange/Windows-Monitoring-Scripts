using System;

namespace Monitis.API.Common
{
    /// <summary>
    /// Couple of simple methods for validation input
    /// </summary>
    public static class Validation
    {
        public static void ValidateAPIKey(String apiKey)
        {
            if (String.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException("apiKey");
            }
        }

        public static void ValidateAuthToken(String authToken)
        {
            if (String.IsNullOrEmpty(authToken))
            {
                throw new ArgumentNullException("authToken");
            }
        }

        public static void ValidateSecretKey(String secretKey)
        {
            if (String.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException("secretKey");
            }
        }
    }
}