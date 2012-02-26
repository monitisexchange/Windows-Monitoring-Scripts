using System;
using Microsoft.WindowsAzure.StorageClient;

namespace Monitis.Prototype.Logic.Azure.TableService
{
    /// <summary>
    /// Class represent entity over table <see cref="WADPerformanceTable"/>
    /// </summary>
    public class PerformanceData : TableServiceEntity, IComparable
    {
        /// <summary>
        /// Tick count for table entry
        /// </summary>
        public Int64 EventTickCount
        {
            get { return _eventTickCount; }
            set { _eventTickCount = value; }
        }

        /// <summary>
        /// Azure deployment ID
        /// </summary>
        public String DeploymentId { get; set; }

        /// <summary>
        /// Azure role name
        /// </summary>
        public String Role { get; set; }

        /// <summary>
        /// Azure role instance
        /// </summary>
        public String RoleInstance { get; set; }

        /// <summary>
        /// Perfomance counter name. Windows style <example>\Processor(_Total)\% Processor Time</example>
        /// </summary>
        public String CounterName { get; set; }

        /// <summary>
        /// Perfomance counter value
        /// </summary>
        public Double CounterValue { get; set; }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return _eventTickCount.CompareTo(((PerformanceData)obj)._eventTickCount);
        }

        #endregion IComparable Members

        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o == null || (o as PerformanceData) == null) return false;

            var that = (PerformanceData)o;

            if (_eventTickCount != that._eventTickCount) return false;
            if (CounterName != null ? !CounterName.Equals(that.CounterName) : that.CounterName != null) return false;
            if (DeploymentId != null ? !DeploymentId.Equals(that.DeploymentId) : that.DeploymentId != null)
                return false;
            if (Role != null ? !Role.Equals(that.Role) : that.Role != null) return false;
            if (RoleInstance != null ? !RoleInstance.Equals(that.RoleInstance) : that.RoleInstance != null)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int result = DeploymentId != null ? DeploymentId.GetHashCode() : 0;
            result = 31 * result + (Role != null ? Role.GetHashCode() : 0);
            result = 31 * result + (RoleInstance != null ? RoleInstance.GetHashCode() : 0);
            result = 31 * result + (CounterName != null ? CounterName.GetHashCode() : 0);
            result = 31 * result + (int)(_eventTickCount ^ (_eventTickCount >> 32));
            return result;
        }

        private Int64 _eventTickCount;
    }
}