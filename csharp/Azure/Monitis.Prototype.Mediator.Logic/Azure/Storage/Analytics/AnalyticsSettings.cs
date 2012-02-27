using System;

namespace Monitis.Prototype.Logic.Azure.Storage.Analytics
{
    /// <summary>
    /// The analytic settings that can set/get
    /// </summary>
    public class AnalyticsSettings
    {
        #region static members

        /// <summary>
        /// Number of analytics version
        /// </summary>
        public static String Version = StorageResource.AnalyticsSettingsVersion;

        #endregion static members

        #region constructors

        /// <summary>
        /// Default constructor with default analytics settings
        /// </summary>
        public AnalyticsSettings()
        {
            LogType = LoggingLevel.None;
            LogVersion = Version;
            IsLogRetentionPolicyEnabled = false;
            LogRetentionInDays = 0;

            MetricsType = MetricsType.None;
            MetricsVersion = Version;
            IsMetricsRetentionPolicyEnabled = false;
            MetricsRetentionInDays = 0;
        }

        #endregion constructors

        #region public properties

        /// <summary>
        /// The type of logs subscribed for
        /// </summary>
        public LoggingLevel LogType { get; set; }

        /// <summary>
        ///  The version of the logs
        /// </summary>
        public String LogVersion { get; set; }

        /// <summary>
        /// Flag indicating if retention policy is set for logs in $logs
        /// </summary>
        public Boolean IsLogRetentionPolicyEnabled { get; set; }

        /// <summary>
        /// The number of days to retain logs for under $logs container
        /// </summary>
        public Int32 LogRetentionInDays { get; set; }

        /// <summary>
        /// The metrics version
        /// </summary>
        public String MetricsVersion { get; set; }

        /// <summary>
        /// A flag indicating if retention policy is enabled for metrics
        /// </summary>
        public Boolean IsMetricsRetentionPolicyEnabled { get; set; }

        /// <summary>
        /// The number of days to retain metrics data
        /// </summary>
        public Int32 MetricsRetentionInDays { get; set; }

        /// <summary>
        /// The type of metrics subscribed for
        /// </summary>
        public MetricsType MetricsType
        {
            get { return _metricsType; }

            set
            {
                if (value == MetricsType.ApiSummary)
                {
                    throw new ArgumentException("Including just ApiSummary is invalid.");
                }
                _metricsType = value;
            }
        }

        #endregion public properties

        #region private fields

        private MetricsType _metricsType = MetricsType.None;

        #endregion private fields
    }
}