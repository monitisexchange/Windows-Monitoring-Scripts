using System;

namespace Monitis.Prototype.Logic.Azure.Storage.Analytics
{
    /// <summary>
    /// The analytic settings that can set/get
    /// </summary>
    public class AnalyticsSettings : IEquatable<AnalyticsSettings>
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

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(AnalyticsSettings other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.LogType, LogType) && Equals(other.LogVersion, LogVersion) && other.IsLogRetentionPolicyEnabled.Equals(IsLogRetentionPolicyEnabled) && other.LogRetentionInDays == LogRetentionInDays && Equals(other.MetricsVersion, MetricsVersion) && other.IsMetricsRetentionPolicyEnabled.Equals(IsMetricsRetentionPolicyEnabled) && other.MetricsRetentionInDays == MetricsRetentionInDays;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AnalyticsSettings)) return false;
            return Equals((AnalyticsSettings) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = LogType.GetHashCode();
                result = (result*397) ^ (LogVersion != null ? LogVersion.GetHashCode() : 0);
                result = (result*397) ^ IsLogRetentionPolicyEnabled.GetHashCode();
                result = (result*397) ^ LogRetentionInDays;
                result = (result*397) ^ (MetricsVersion != null ? MetricsVersion.GetHashCode() : 0);
                result = (result*397) ^ IsMetricsRetentionPolicyEnabled.GetHashCode();
                result = (result*397) ^ MetricsRetentionInDays;
                return result;
            }
        }

        public static bool operator ==(AnalyticsSettings left, AnalyticsSettings right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AnalyticsSettings left, AnalyticsSettings right)
        {
            return !Equals(left, right);
        }
    }
}