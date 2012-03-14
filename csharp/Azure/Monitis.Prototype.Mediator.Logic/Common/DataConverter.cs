using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Monitis.API.Domain.Monitors;
using Monitis.Prototype.Logic.Azure.TableService;
using Monitis.Prototype.Logic.Monitis;

namespace Monitis.Prototype.Logic.Common
{
    /// <summary>
    /// Helper methods for convert data types
    /// </summary>
    public static class DataConverter
    {
        /// <summary>
        /// Convert Windows Azure perfomance counter data to Monitis monitor <see cref="ResultParameter"/>
        /// </summary>
        /// <param name="data">Data to convert</param>
        /// <param name="convert">Custom convertion function</param>
        /// <returns></returns>
        public static List<ResultParameter> ConvertToMonitorResults(IEnumerable<PerformanceData> data,
                                                                     Func<Double, String> convert)
        {
            var monitorData = new List<ResultParameter>();
            foreach (PerformanceData performanceData in data)
            {
                String monitisParameterName = UserSession.GetMonitisParameterName(performanceData.CounterName);

                ResultParameter parameter = ConvertToMonitorResult(performanceData.CounterValue,
                                                                   performanceData.Timestamp, convert,
                                                                   monitisParameterName);
                monitorData.Add(parameter);
            }
            return monitorData;
        }

        /// <summary>
        /// Convert value to parameter which understand monitis.com 
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="timeStamp">Parameter timestamp</param>
        /// <param name="convert">Custom converter function</param>
        /// <param name="resultParameterName">Parameter name</param>
        /// <returns></returns>
        public static ResultParameter ConvertToMonitorResult(Double value, DateTime timeStamp,
                                                             Func<Double, String> convert, String resultParameterName)
        {
            String convertedValue = convert == null
                                        ? value.ToString()
                                        : convert(value);
            return new ResultParameter
            {
                Name = HttpUtility.UrlEncode(resultParameterName),
                Value = HttpUtility.UrlEncode(convertedValue),
                Timestamp = timeStamp
            };
        }

        /// <summary>
        /// Convert value to parameter which understand monitis.com 
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="timeStamp">Parameter timestamp</param>
        /// <param name="config">Configuration of monitor for convert details</param>
        /// <returns></returns>
        public static ResultParameter ConvertToMonitorResult(Double value, DateTime timeStamp,
                                                             CustomMonitorConfig config)
        {
            return ConvertToMonitorResult(value, timeStamp, config.Descriptor.Converter,
                                          config.Descriptor.ResultParams.First().Name);
        }
    }
}