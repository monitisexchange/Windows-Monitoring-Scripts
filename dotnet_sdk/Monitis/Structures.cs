using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Monitis.Structures
{

    #region Additional classes for deserialize Json responses

    [Serializable]
    public struct AlertJson
    {
        /// <summary>
        /// Contains status field
        /// </summary>
        public string status;

        public Alert[] data;
    }

    public struct TestJson
    {
        public Test[] testList;
    }

    public struct TagsExternalMonitorJson
    {
        public Tag[] tags;
    }

    #endregion

    [Serializable]
    [XmlType(TypeName = Params.contact)]
    public struct Contact
    {
        public int contactId;
        public int newsletterFlag;
        public string name;
        public Monitis.Contact.ContactType contactType;
        public string contactAccount;
        public int timezone;
        public bool portable;
        public int activeFlag;
        public int textType;
        public int confirmationFlag;
        public string country;
    }

    [Serializable]
    [XmlType(TypeName = Params.group)]
    public struct ContactGroup
    {
        public int id;
        public int activeFlag;
        public string name;
    }

    [Serializable]
    [XmlType(TypeName = Params.alert)]
    public struct Alert
    {
        /// <summary>
        /// type of the monitor 
        /// </summary>
        public string dataType;

        /// <summary>
        /// time of the recovery 
        /// </summary>
        public DateTime recDate;

        /// <summary>
        /// id of the monitor 
        /// </summary>
        public int dataId;

        /// <summary>
        /// time of the failure 
        /// </summary>
        public DateTime failDate;

        /// <summary>
        /// id of the monitor type 
        /// </summary>
        public int dataTypeId;

        /// <summary>
        /// comma separated contact accounts, that alerts have been sent to 
        /// TODO: separate to array?
        /// </summary>
        public string contacts;

        /// <summary>
        /// name of the monitor 
        /// </summary>
        public string dataName;
    }

    [Serializable]
    [XmlType(TypeName = Params.subaccount)]
    public struct SubAccount
    {
        /// <summary>
        /// id of the sub account
        /// </summary>
        public int id;

        /// <summary>
        /// sub account
        /// </summary>
        public string account;

        /// <summary>
        /// first name of the sub account owner
        /// </summary>
        public string firstName;

        /// <summary>
        /// last name of the sub account owner
        /// </summary>
        public string lastName;

        /// <summary>
        /// user key of the sub account
        /// </summary>
        public string userkey;
    }

    [Serializable]
    [XmlType(TypeName = Params.subaccountpage)]
    public struct SubAccountPage
    {
        /// <summary>
        /// id of the sub account 
        /// </summary>
        public int id;

        /// <summary>
        /// username of the sub account 
        /// </summary>
        public string account;

        /// <summary>
        /// contains names of pages of the sub account 
        /// </summary>
        [XmlArray(Params.pages)] [XmlArrayItem(Params.page)] public string[] pages;
    }

    [Serializable]
    [XmlType(TypeName = Params.page)]
    public struct Page
    {
        /// <summary>
        /// id of the page
        /// </summary>
        public int id;

        /// <summary>
        /// title of the page
        /// </summary>
        public string title;
    }

    [Serializable]
    [XmlType(TypeName = Params.pageModule)]
    public struct PageModule
    {
        /// <summary>
        /// id of the page module
        /// </summary>
        public int id;

        /// <summary>
        /// name of the module
        /// </summary>
        public string moduleName;

        /// <summary>
        /// id of the test which results are shown in the module
        /// </summary>
        public int dataModuleId;
    }

    [Serializable]
    [XmlType(TypeName = Params.test)]
    public struct Test
    {
        /// <summary>
        /// id to identify the test
        /// </summary>
        [XmlAttribute] public int id;

        /// <summary>
        /// name of the test
        /// </summary>
        [XmlText] public string name;

        /// <summary>
        /// if 1 monitor is suspended
        /// </summary>
        [XmlAttribute] public int isSuspended;

        /// <summary>
        /// type of the monitor
        /// </summary>
        [XmlAttribute] public string type;
    }

    public struct AddExternalMonitorResponse
    {
        public DateTime startDate;
        public int testId;
        public int isTestNew;
    }

    [Serializable]
    [XmlType(TypeName = Params.tag)]
    public struct Tag
    {
        /// <summary>
        /// count of the user's tests with this tag
        /// </summary>
        [XmlAttribute] public int rank;

        /// <summary>
        /// name of the tag
        /// </summary>
        [XmlText] public string title;
    }

    [Serializable]
    [XmlType(TypeName = Params.location)]
    public struct Location
    {
        /// <summary>
        /// name of the location
        /// </summary>
        public string name;

        /// <summary>
        /// identifier of the location
        /// </summary>
        public int id;

        /// <summary>
        /// minimal check interval for user's external monitors in minutes
        /// </summary>
        public ExternalMonitor.CheckInterval minCheckInterval;
    }

    [Serializable]
    [XmlType(TypeName = Params.location)]
    public struct LocationWithMonitorsResults
    {
        /// <summary>
        /// full name of the location the following results are received for
        /// </summary>
        [XmlAttribute]
        public string name /*Name for json here*/;

        /// <summary>
        /// id of the location the following results are received for
        /// </summary>
        [XmlAttribute] public int id;

        /// <summary>
        /// Contains test data
        /// </summary>
        [XmlElement(Params.test, typeof(SnapshotData)/*to parse array without parent element ("tests")*/)]
        public SnapshotData[] data  /*name for json here*/;
    }

    public struct SnapshotData
    {
        /// <summary>
        /// time of the check
        /// </summary>
        [XmlIgnore]
        public DateTime time;

        [XmlElement(ElementName = Params.time)]
        public string timeString /*fix - incorrect response of datetime from server*/
        {
            get { return time.ToString("s"); }
            set { time = DateTime.Parse(value); }
        }

        /// <summary>
        /// tag of the test
        /// </summary>
        public string tag;

        /// <summary>
        /// response time in ms
        /// </summary>
        [XmlElement(ElementName = Params.performance /*Name for XML here*/)] public float perf; /*name for json here*/

        /// <summary>
        /// status - test status. Possible values are 0-OK, 1-warning, 2-NOK, null-NA.
        /// </summary>
        public ExternalMonitor.Status? status; //TODO: 0, 1, 2 - parse if XML

        /// <summary>
        /// the name of the test
        /// </summary>
        public string name;

        /// <summary>
        /// id of the test
        /// </summary>
        public int id;

        /// <summary>
        /// timeout - test timeout
        /// </summary>
        public int timeout;
    }

    [DataContract(Name = Params.result, Namespace = "")]
    public struct ExternalMonitorInformation
    {
        /// <summary>
        /// test timeout in ms
        /// </summary>
        [DataMember] 
        public int timeout;

        /// <summary>
        /// creation date of the test
        /// </summary>
        [DataMember]
        [XmlIgnore/*Datetime is in incorrect format*/]
        public DateTime startDate;

        [XmlElement(ElementName = Params.startDate)]
        public string startDateString
        {
            get { return startDate.ToString("s"); }
            set { startDate = DateTime.Parse(value); }
        }

        /// <summary>
        /// Test type
        /// </summary>
        [DataMember] public ExternalMonitor.TestType type;

        /// <summary>
        /// This is actual if test is of type POST. It is the data sent during the post request. 
        /// </summary>
        [DataMember] public string postData;

        /// <summary>
        /// id of the test
        /// </summary>
        [DataMember] public int testId;

        [XmlIgnore]
        public bool? match;

        /// <summary>
        /// the value is 1 if there is string to match in response text otherwise it is 0.
        /// </summary>
        [DataMember]
        [XmlElement(ElementName = Params.match)]
        public string matchString
        {
            get
            {
                return match.ToString();
            }
            set
            {
                if (null != value&&value!="null")
                    match = bool.Parse(value);
                else
                {
                    match = null;
                }
            }
        }

        /// <summary>
        /// text to match in the response
        /// </summary>
        [DataMember] public string matchText;

        ///<summary>
        ///additional test parametrs (e.g. username and password for mysql test)
        ///</summary>
        [DataMember]
        [XmlIgnore/*TODO: delete if the way to replace dictonary will be found*/]
        public Dictionary<string,string> @params;

        /// <summary>
        /// tag of the test
        /// </summary>
        [DataMember] public string tag;

        /// <summary>
        /// is actual for HTTP test, specifies the request method
        /// </summary>
        [DataMember] public ExternalMonitor.DetailedTestType? detailedType;

        /// <summary>
        /// url of the test
        /// </summary>
        [DataMember] public string url;

        /// <summary>
        /// name of the test
        /// </summary>
        [DataMember] public string name;

        /// <summary>
        /// Locations
        /// </summary>
        [XmlArrayItem(ElementName = Params.location)]
        [DataMember] public LocationInMonitorInfo[] locations;
    }


    public struct LocationInMonitorInfo
    {
        /// <summary>
        ///  id of the monitoring location
        /// </summary>
        public int id;

        /// <summary>
        /// interval of checks for this location in minutes
        /// </summary>
        public int checkInterval;

        /// <summary>
        /// full name of the location(e.g Panama, Australia, Germany ...)
        /// </summary>
        public string fullName;

        /// <summary>
        /// name of the location(e.g PA, AU, DE, ...)
        /// </summary>
        public string name;
    }

    public struct ExternalMonitorTop
    {
        [XmlArrayItem(ElementName = Params.test)]
        public ExternalMonitorTest[] tests;
    }

    public struct ExternalMonitorTest
    {
        public int id;
        public float result;
        public string testName;
        [XmlIgnore]
        public TimeSpan lastCheckTime;

        [XmlElement(ElementName = Params.lastCheckTime)]
        public string lastCheckTimeString
        {
            get { return lastCheckTime.ToString(); }
            set { lastCheckTime = TimeSpan.Parse(value); }
        }

        public string status;
    }
}
