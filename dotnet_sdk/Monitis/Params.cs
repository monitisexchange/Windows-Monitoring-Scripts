using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monitis
{
    //internal after test
    public static class Params
    {
        public const string action = "action";
        public const string apikey = "apikey";
        public const string timestamp = "timestamp";
        public const string version = "version";
        public const string checksum = "checksum";
        public const string authToken = "authToken";
        public const string output = "output";
        public const string userName = "userName";
        public const string password = "password";
        public const string secretkey = "secretkey";
        public const string validation = "validation";

        /// <summary>
        /// "error" responses from server - for detecting errors
        /// </summary>
        public const string error = "error";

        public const string confirmationKey = "confirmationKey";
        public const string contactId = "contactId";
        public const string account = "account";
        public const string contactType = "contactType";
        public const string code = "code";
        public const string portable = "portable";
        public const string textType = "textType";
        public const string country = "country";
        public const string timezone = "timezone";
        public const string @group = "group";
        public const string lastName = "lastName";
        public const string firstName = "firstName";
        public const string limit = "limit";
        public const string endDate = "endDate";
        public const string startDate = "startDate";
        public const string email = "email";
        public const string userId = "userId";
        public const string pageNames = "pageNames";

        /// <summary>
        /// Root element in xml response Contact.GetContacts method
        /// </summary>
        public const string contacts = "contacts";
        public const string contact = "contact";

        /// <summary>
        /// Root element in xml response Contact.GetContactGroups method
        /// </summary>
        public const string contactgroups = "contactgroups";

        /// <summary>
        /// Root element in xml response Contact.GetRecentAlerts method
        /// </summary>
        public const string recentAlerts = "recentAlerts";

        public const string result = "result";
        public const string ok = "ok";
        public const string status = "status";
        public const string subaccounts = "subaccounts";
        public const string subaccount = "subaccount";
        public const string data = "data";
        public const string subaccountpage = "subaccountpage";
        public const string pages = "pages";
        public const string page = "page";
        public const string alert = "alert";
        public const string pageName = "pageName";
        public const string title = "title";
        public const string columnCount = "columnCount";
        public const string pageId = "pageId";
        public const string pageModuleId = "pageModuleId";
        public const string moduleName = "moduleName";
        public const string row = "row";
        public const string column = "column";
        public const string height = "height";
        public const string dataModuleId = "dataModuleId";
        public const string pageModule = "pageModule";
        public const string pageModules = "pageModules";
        public const string monitorId = "monitorId";
        public const string monitorIds = "monitorIds";
        public const string detailedResults = "detailedResults";
        public const string tag = "tag";
        public const string year = "year";
        public const string month = "month";
        public const string day = "day";
        public const string name = "name";
        public const string url = "url";
        public const string type = "type";
        public const string interval = "interval";
        public const string contentMatchString = "contentMatchString";
        public const string contentMatchFlag = "contentMatchFlag";
        public const string basicAuthUser = "basicAuthUser";
        public const string basicAuthPass = "basicAuthPass";
        public const string postData = "postData";
        public const string @params = "params";
        public const string detailedTestType = "detailedTestType";
        public const string timeout = "timeout";
        public const string uptimeSLA = "uptimeSLA";
        public const string responseSLA = "responseSLA";
        public const string testIds = "testIds";
        public const string testId = "testId";
        public const string locationIds = "locationIds";
        public const string maxValue = "maxValue";
        public const string overSSL = "overSSL";
        public const string test = "test";
        public const string isTestNew = "isTestNew";
        public const string location = "location";
        public const string locations = "locations";
        public const string performance = "performance";
        public const string time = "time";
        public const string lastCheckTime = "lastCheckTime";
        public const string match = "match";
    }
}
