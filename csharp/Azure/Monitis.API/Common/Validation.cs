using System;

namespace Monitis.API.Common
{
    /// <summary>
    /// Couple of simple methods for validation input
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Validate string value for empty or null. If detected throws exception <see cref="ArgumentNullException"/>
        /// </summary>
        /// <param name="value">Value for validate</param>
        /// <param name="argName">Argument name</param>
        public static void EmptyOrNull(String value, String argName)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(argName);
            }
        }
    }
}