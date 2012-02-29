/*
 
 code sample from Windows Azure Storage blog 
 http://blogs.msdn.com/b/windowsazurestorage/archive/2011/08/03/windows-azure-storage-metrics-using-metrics-to-track-storage-usage.aspx
 
  */

using System;
using System.Xml;

namespace Monitis.Prototype.Logic.Azure.Storage.Analytics
{
    public class SettingsSerializerHelper
    {
        private const string RootPropertiesElementName = "StorageServiceProperties";
        private const string VersionElementName = "Version";
        private const string RetentionPolicyElementName = "RetentionPolicy";
        private const string RetentionPolicyEnabledElementName = "Enabled";
        private const string RetentionPolicyDaysElementName = "Days";

        private const string LoggingElementName = "Logging";
        private const string ApiTypeDeleteElementName = "Delete";
        private const string ApiTypeReadElementName = "Read";
        private const string ApiTypeWriteElementName = "Write";

        private const string MetricsElementName = "Metrics";
        private const string IncludeApiSummaryElementName = "IncludeAPIs";
        private const string MetricsEnabledElementName = "Enabled";

        private const int MaximumRetentionDays = 365;

        /// <summary>
        /// Reads the settings provided from stream 
        /// </summary> 
        /// <param name="xmlReader"></param> 
        /// <returns></returns> 
        public static AnalyticsSettings DeserializeAnalyticsSettings(XmlReader xmlReader)
        {
            // Read the root and check if it is empty or invalid xmlReader.Read();
            xmlReader.ReadStartElement(SettingsSerializerHelper.RootPropertiesElementName);

            AnalyticsSettings settings = new AnalyticsSettings();

            while (true)
            {
                if (xmlReader.IsStartElement(SettingsSerializerHelper.LoggingElementName))
                {
                    DeserializeLoggingElement(xmlReader, settings);
                }
                else if (xmlReader.IsStartElement(SettingsSerializerHelper.MetricsElementName))
                {
                    DeserializeMetricsElement(xmlReader, settings);
                }
                else
                {
                    break;
                }
            }

            xmlReader.ReadEndElement();

            return settings;
        }


        /// <summary> 
        /// Write the settings provided to stream 
        /// </summary> 
        /// <param name="inputStream"></param> 
        /// <returns></returns> 
        public static void SerializeAnalyticsSettings(XmlWriter xmlWriter, AnalyticsSettings settings)
        {
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement(SettingsSerializerHelper.RootPropertiesElementName);

            //LOGGING STARTS HERE 
            xmlWriter.WriteStartElement(SettingsSerializerHelper.LoggingElementName);

            xmlWriter.WriteStartElement(SettingsSerializerHelper.VersionElementName);
            xmlWriter.WriteValue(settings.LogVersion);
            xmlWriter.WriteEndElement();

            bool isReadEnabled = (settings.LogType & LoggingLevel.Read) != LoggingLevel.None;
            xmlWriter.WriteStartElement(SettingsSerializerHelper.ApiTypeReadElementName);
            xmlWriter.WriteValue(isReadEnabled);
            xmlWriter.WriteEndElement();

            bool isWriteEnabled = (settings.LogType & LoggingLevel.Write) != LoggingLevel.None;
            xmlWriter.WriteStartElement(SettingsSerializerHelper.ApiTypeWriteElementName);
            xmlWriter.WriteValue(isWriteEnabled);
            xmlWriter.WriteEndElement();

            bool isDeleteEnabled = (settings.LogType & LoggingLevel.Delete) != LoggingLevel.None;
            xmlWriter.WriteStartElement(SettingsSerializerHelper.ApiTypeDeleteElementName);
            xmlWriter.WriteValue(isDeleteEnabled);
            xmlWriter.WriteEndElement();

            SerializeRetentionPolicy(xmlWriter, settings.IsLogRetentionPolicyEnabled, settings.LogRetentionInDays);
            xmlWriter.WriteEndElement(); // logging element 
            
            //METRICS STARTS HERE 
            xmlWriter.WriteStartElement(SettingsSerializerHelper.MetricsElementName);

            xmlWriter.WriteStartElement(SettingsSerializerHelper.VersionElementName);
            xmlWriter.WriteValue(settings.MetricsVersion);
            xmlWriter.WriteEndElement();

            bool isServiceSummaryEnabled = (settings.MetricsType & MetricsType.ServiceSummary) != MetricsType.None;
            xmlWriter.WriteStartElement(SettingsSerializerHelper.MetricsEnabledElementName);
            xmlWriter.WriteValue(isServiceSummaryEnabled);
            xmlWriter.WriteEndElement();

            if (isServiceSummaryEnabled)
            {
                bool isApiSummaryEnabled = (settings.MetricsType & MetricsType.ApiSummary) != MetricsType.None;
                xmlWriter.WriteStartElement(SettingsSerializerHelper.IncludeApiSummaryElementName);
                xmlWriter.WriteValue(isApiSummaryEnabled);
                xmlWriter.WriteEndElement();
            }

            SerializeRetentionPolicy(
                xmlWriter,
                settings.IsMetricsRetentionPolicyEnabled,
                settings.MetricsRetentionInDays);
            xmlWriter.WriteEndElement(); 
            // metrics 
            xmlWriter.WriteEndElement(); 
            // root element 
            xmlWriter.WriteEndDocument();
        }

        private static void SerializeRetentionPolicy(XmlWriter xmlWriter, Boolean isRetentionEnabled, Int32 days)
        {
            xmlWriter.WriteStartElement(SettingsSerializerHelper.RetentionPolicyElementName);

            xmlWriter.WriteStartElement(SettingsSerializerHelper.RetentionPolicyEnabledElementName);
            xmlWriter.WriteValue(isRetentionEnabled);
            xmlWriter.WriteEndElement();

            if (isRetentionEnabled)
            {
                xmlWriter.WriteStartElement(SettingsSerializerHelper.RetentionPolicyDaysElementName);
                xmlWriter.WriteValue(days);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement(); // Retention policy for logs
        }

        /// <summary> 
        ///  Reads the logging element and fills in the values in Analyticssettings instance 
        /// </summary> 
        /// <param name="xmlReader"></param> 
        /// <param name="settings"></param> 
        private static void DeserializeLoggingElement(XmlReader xmlReader, AnalyticsSettings settings)
        {
            // Read logging element 
            xmlReader.ReadStartElement(SettingsSerializerHelper.LoggingElementName);

            while (true)
            {
                if (xmlReader.IsStartElement(SettingsSerializerHelper.VersionElementName))
                {
                    settings.LogVersion = xmlReader.ReadElementString(SettingsSerializerHelper.VersionElementName);
                }
                else if (xmlReader.IsStartElement(SettingsSerializerHelper.ApiTypeReadElementName))
                {
                    if (DeserializeBooleanElementValue(
                        xmlReader,
                        SettingsSerializerHelper.ApiTypeReadElementName))
                    {
                        settings.LogType = settings.LogType | LoggingLevel.Read;
                    }
                }
                else if (xmlReader.IsStartElement(SettingsSerializerHelper.ApiTypeWriteElementName))
                {
                    if (DeserializeBooleanElementValue(
                        xmlReader,
                        SettingsSerializerHelper.ApiTypeWriteElementName))
                    {
                        settings.LogType = settings.LogType | LoggingLevel.Write;
                    }
                }
                else if (xmlReader.IsStartElement(SettingsSerializerHelper.ApiTypeDeleteElementName))
                {
                    if (DeserializeBooleanElementValue(
                        xmlReader,
                        SettingsSerializerHelper.ApiTypeDeleteElementName))
                    {
                        settings.LogType = settings.LogType | LoggingLevel.Delete;
                    }
                }
                else if (xmlReader.IsStartElement(SettingsSerializerHelper.RetentionPolicyElementName))
                {
                    // read retention policy for logging 
                    bool isRetentionEnabled = false;
                    int retentionDays = 0;
                    DeserializeRetentionPolicy(xmlReader, ref isRetentionEnabled, ref retentionDays);
                    settings.IsLogRetentionPolicyEnabled = isRetentionEnabled;
                    settings.LogRetentionInDays = retentionDays;
                }
                else
                {
                    break;
                }
            }

            xmlReader.ReadEndElement();// end Logging element 
        }

        /// <summary> 
        /// Reads the metrics element and fills in the values in Analyticssettings instance
        /// </summary> 
        /// <param name="xmlReader"></param> 
        /// <param name="settings"></param> 
        private static void DeserializeMetricsElement(XmlReader xmlReader, AnalyticsSettings settings)
        {
            bool includeAPIs = false;

            // read the next element - it should be metrics. 
            xmlReader.ReadStartElement(SettingsSerializerHelper.MetricsElementName);

            while (true)
            {
                if (xmlReader.IsStartElement(SettingsSerializerHelper.VersionElementName))
                {
                    settings.MetricsVersion = xmlReader.ReadElementString(SettingsSerializerHelper.VersionElementName);
                }
                else if (xmlReader.IsStartElement(SettingsSerializerHelper.MetricsEnabledElementName))
                {
                    if (DeserializeBooleanElementValue(
                        xmlReader,
                        SettingsSerializerHelper.MetricsEnabledElementName))
                    {
                        // only if metrics is enabled will we read include API 
                        settings.MetricsType = settings.MetricsType | MetricsType.ServiceSummary;
                    }
                }
                else if (xmlReader.IsStartElement(SettingsSerializerHelper.IncludeApiSummaryElementName))
                {
                    if (DeserializeBooleanElementValue(
                        xmlReader,
                        SettingsSerializerHelper.IncludeApiSummaryElementName))
                    {
                        includeAPIs = true;
                    }
                }
                else if (xmlReader.IsStartElement(SettingsSerializerHelper.RetentionPolicyElementName))
                {
                    // read retention policy for metrics 
                    bool isRetentionEnabled = false;
                    int retentionDays = 0;
                    DeserializeRetentionPolicy(xmlReader, ref isRetentionEnabled, ref retentionDays);
                    settings.IsMetricsRetentionPolicyEnabled = isRetentionEnabled;
                    settings.MetricsRetentionInDays = retentionDays;
                }
                else
                {
                    break;
                }
            }

            if ((settings.MetricsType & MetricsType.ServiceSummary) != MetricsType.None)
            {
                // If Metrics is enabled, IncludeAPIs must be included. 
                if (includeAPIs)
                {
                    settings.MetricsType = settings.MetricsType | MetricsType.ApiSummary;
                }
            }

            xmlReader.ReadEndElement();// end metrics element 
        }


        /// <summary>
        ///  Reads the retention policy in logging and metrics elements and fills in the values in Analyticssettings instance.
        /// </summary> 
        /// <param name="xmlReader"></param>
        /// <param name="isRetentionEnabled"></param> 
        /// <param name="retentionDays"></param> 
        private static void DeserializeRetentionPolicy(XmlReader xmlReader, ref bool isRetentionEnabled, ref int retentionDays)
        {
            xmlReader.ReadStartElement(SettingsSerializerHelper.RetentionPolicyElementName);

            while (true)
            {
                if (xmlReader.IsStartElement(SettingsSerializerHelper.RetentionPolicyEnabledElementName))
                {
                    isRetentionEnabled = DeserializeBooleanElementValue(
                        xmlReader,
                        SettingsSerializerHelper.RetentionPolicyEnabledElementName);
                }
                else if (xmlReader.IsStartElement(SettingsSerializerHelper.RetentionPolicyDaysElementName))
                {
                    string intValue = xmlReader.ReadElementString(
                        SettingsSerializerHelper.RetentionPolicyDaysElementName);
                    retentionDays = int.Parse(intValue);
                }
                else
                {
                    break;
                }
            }

            xmlReader.ReadEndElement(); // end reading retention policy
        }

        /// <summary> Read a boolean value for xml element
        /// </summary> /// <param name="xmlReader"></param>
        /// <param name="elementToRead"></param> 
        /// <returns></returns> 
        private static bool DeserializeBooleanElementValue(XmlReader xmlReader, String elementToRead)
        {
            string boolValue = xmlReader.ReadElementString(elementToRead);
            return bool.Parse(boolValue);
        }
    }
}