using System;
using System.Linq;
using System.Web;
using Monitis.API.Domain.Monitors;

namespace Monitis.API.Util
{
    /// <summary>
    /// Utils methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        ///  January 1, 1970, 00:00:00 GMT
        /// </summary>
        private static readonly DateTime LinuxStartTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// Join monitor parameters description into single line with encoding
        /// </summary>
        /// <param name="parametersDescriptors">Parameters description array</param>
        /// <returns></returns>
        public static String JoinMonitorResultParams(ResultParameterDescriptor[] parametersDescriptors)
        {
            //join by pattern : name1:displayName1:uom1:dataType1[;name2:displayName2:uom2:dataType2...]
            String joinedParameters = parametersDescriptors
                .Aggregate(String.Empty, 
                           (current, parameter) => current + String.Format("{0}:{1}:{2}:{3};", 
                               parameter.Name, parameter.DisplayName,
                               HttpUtility.UrlEncode(parameter.UOM), (Int32) parameter.DataType));
            return HttpUtility.UrlEncode(joinedParameters);
        }

        /// <summary>
        /// Substract Linuinput datetime value Return checktime value in milliseconds
        /// </summary>
        /// <param name="fromTime">Base time of </param>
        /// <returns>Milliseconds value</returns>
        public static String GetMillisecondsCheckTime(DateTime fromTime)
        {
            Double milliseconds = (fromTime - LinuxStartTime).TotalMilliseconds;
            Int64 checktime = (Int64)milliseconds;
            return checktime.ToString();
        }
    }
}