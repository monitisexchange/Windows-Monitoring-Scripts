using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Monitis
{
    public class Contact : APIObject
    {
        /// <summary>
        /// Types of contact (from doc)
        /// </summary>
        public enum ContactType
        {
            Email = 1,
            SMS = 2,
            ICQ = 3,
            Google = 7,
            Twitter = 8,
            PhoneCall = 9,
            SMSCall = 10,
            URL = 11
        }

        /// <summary>
        ///  Action names (from doc)
        /// </summary>
        private enum ContactAction
        {
            contactsList,
            contactGroupList,
            recentAlerts,
            addContact,
            editContact,
            deleteContact,
            confirmContact,
            contactActivate,
            contactDeactivate
        }

        /// <summary>
        /// Constructor without params. Dont forget to set tokens!
        /// </summary>
        public Contact()
        {

        }

        public Contact(string apiKey, string secretKey)
            : base(apiKey, secretKey)
        {

        }

        public Contact(string apiKey, string secretKey, string apiUrl)
            : base(apiKey, secretKey, apiUrl)
        {

        }

        public Contact(Authentication authentication)
            : base(authentication)
        {

        }

        /// <summary>
        /// This action is used to get user's all contacts. 
        /// </summary>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <returns>Contacts of current user</returns>
        public virtual Structures.Contact[] GetContacts(OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            Structures.Contact[] contacts = null;
            RestResponse response = MakeGetRequest(ContactAction.contactsList, output: outputType);
            contacts = Helper.DeserializeObject<Structures.Contact[]>(response, outputType, Params.contacts);
            return contacts;
        }


        /// <summary>
        /// This action is used to get all groups of contacts of the user. 
        /// </summary>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <returns>List of contact groups</returns>
        public virtual Structures.ContactGroup[] GetContactGroups(OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            Structures.ContactGroup[] contactGroups = null;
            var parameters = new Dictionary<string, object>();
            RestResponse response = MakeGetRequest(ContactAction.contactGroupList, parameters, outputType);
            contactGroups = Helper.DeserializeObject<Structures.ContactGroup[]>(response, outputType,
                                                                                Params.contactgroups);
            return contactGroups;
        }

        /// <summary>
        /// This action is used to get recent alerts history. 
        /// TODO: add alerts and test
        /// </summary>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="timezone">offset relative to GMT, used to retrieve results in the given timezone. The default value is 0</param>
        /// <param name="startDate">start date to get results for </param>
        /// <param name="endDate">last date to get results for </param>
        /// <param name="limit">number of alerts to get</param>
        /// <returns>recent alerts</returns>
        public virtual Structures.Alert[] GetRecentAlerts(int? timezone = null, long? startDate = null,
                                                          long? endDate = null, int? limit = null,
                                                          OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            Structures.Alert[] alerts = null;
            var parameters = new Dictionary<string, object>();
            AddIfNotNull(parameters, Params.timezone, timezone);
            AddIfNotNull(parameters, Params.startDate, startDate);
            AddIfNotNull(parameters, Params.endDate, endDate);
            AddIfNotNull(parameters, Params.limit, limit);
            RestResponse response = MakeGetRequest(ContactAction.recentAlerts, parameters, outputType);
            //Different responses in XML and JSON, additional check
            if (outputType == OutputType.XML)
                alerts = Helper.Xml.DeserializeObject<Structures.Alert[]>(response, Params.recentAlerts);
            else if (outputType == OutputType.JSON)
            {
                Structures.AlertJson alertsJson = Helper.Json.DeserializeObject<Structures.AlertJson>(response);
                //TODO: Check alertsJson.status and throw exception
                Helper.CheckStatus(response, outputType);
                alerts = alertsJson.data;
            }
            return alerts;
        }

        /// <summary>
        /// This action is used to add a new contact. 
        /// </summary>
        /// <param name="firstName">first name of the contact </param>
        /// <param name="lastName">last name of the contact</param>
        /// <param name="account">account information</param>
        /// <param name="group">the group to which contact will be added.
        ///  A new group will be created if a group with such name doesn’t exist. </param>
        /// <param name="country">full name, 2 or 3 letter codes for the country. E.g. United States, US or USA</param>
        /// <param name="contactType">type of contact (Email, ICQ, etc.)</param>
        /// <param name="timezone">timezone offset from GMT in minute</param>
        /// <param name="textType">could be "true" to get plain text alerts or "false" to get HTML formatted alerts.</param>
        /// <param name="portable">is available only for "SMS" and "SMS and Call" contact types.
        ///  "true" if mobile number was moved from one operator to another under the 'number portability' system.</param>
        /// <param name="sendDailyReport">set to "true" to enable daily reports</param>
        /// <param name="sendWeeklyReport">	set to "true" to enable weekly reports</param>
        /// <param name="sendMonthlyReport">set to "true" to enable monthly reports</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        /// <returns>confirmationKey (as key) and contactId (as value) in KeyValuePair</returns>
        public virtual KeyValuePair<string,int> AddContact(string firstName, string lastName, string account,
                                                        ContactType contactType, int timezone = 0, string country = null,
                                                        bool? textType = null, bool? portable = null,
                                                        string group = null,
                                                        bool? sendDailyReport = null, bool? sendWeeklyReport = null,
                                                        bool? sendMonthlyReport = null, OutputType? output = null,
                                                        Validation? validation = null)
        {
            KeyValuePair<string, int> result;
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.firstName, firstName);
            parameters.Add(Params.lastName, lastName);
            parameters.Add(Params.account, account);
            parameters.Add(Params.contactType, (int) contactType); /*int type required*/
            parameters.Add(Params.timezone, timezone);
            AddIfNotNull(parameters, Params.group, group);
            AddIfNotNull(parameters, Params.country, country);
            AddIfNotNull(parameters, Params.textType, textType);
            AddIfNotNull(parameters, Params.portable, portable);

            RestResponse response = MakePostRequest(ContactAction.addContact, parameters, outputType,
                                                    validation: validation);
            Helper.CheckStatus(response, outputType);
            string confirmationKey = Helper.GetValueOfKey(response, Params.confirmationKey, outputType);
            string contactIdString = Helper.GetValueOfKey(response, Params.contactId, outputType);
            int contactId=Int32.Parse(contactIdString);

            result=new KeyValuePair<string, int>(confirmationKey,contactId);
            return result;
        }

        /// <summary>
        /// This action is used for contact editing. 
        /// </summary>
        /// <param name="contactId">id of the contact to edit</param>
        /// <param name="firstName">first name of the contact</param>
        /// <param name="lastName">last name of the contact</param>
        /// <param name="account">specifies actual account information</param>
        /// <param name="group"></param>
        /// <param name="country">full name, 2 or 3 letter codes for the country. E.g. United States, US or USA</param>
        /// <param name="contactType">type of contact (Email, ICQ, etc.)</param>
        /// <param name="timezone">timezone offset from GMT in minutes</param>
        /// <param name="textType">could be "true" to get plain text alerts or "false" to get HTML formatted alerts.</param>
        /// <param name="portable">Is available only for "SMS" and "SMS and Call" contact types. 
        /// "true" if mobile number was moved from one operator to another under the 'number portability' system.</param>
        /// <param name="code">you can confirm your contact by making "edit contact" API call and by puting confirmation code in this parameter.</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        /// <returns>confirmationKey - key for confirmation of this contact. Used in confirmContact API action</returns>
        public virtual string EditContact(int contactId, string firstName = null, string lastName = null,
                                                          string account = null,
                                                          string group = null, string country = null,
                                                          ContactType? contactType = null, int? timezone = null,
                                                          bool? textType = null, bool? portable = null,
                                                          string code = null, OutputType? output = null,
                                                          Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.contactId, contactId);
            AddIfNotNull(parameters, Params.firstName, firstName);
            AddIfNotNull(parameters, Params.lastName, lastName);
            AddIfNotNull(parameters, Params.account, account);
            if (contactType != null)
                AddIfNotNull(parameters, Params.contactType, (int) contactType); /*int type required*/
            AddIfNotNull(parameters, Params.group, group);
            AddIfNotNull(parameters, Params.timezone, timezone);
            AddIfNotNull(parameters, Params.country, country);
            AddIfNotNull(parameters, Params.textType, textType);
            AddIfNotNull(parameters, Params.portable, portable);
            AddIfNotNull(parameters, Params.code, code);
            RestResponse response = MakePostRequest(ContactAction.editContact, parameters, outputType,
                                                    validation: validation);
            Helper.CheckStatus(response, outputType);
            string confirmationKey = Helper.GetValueOfKey(response, Params.confirmationKey, outputType);
            return confirmationKey;
        }

        /// <summary>
        /// This action is used for contact deleting. 
        /// </summary>
        /// <param name="contactId">id of the contact to delete</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void DeleteContact(int contactId, OutputType? output = null, Validation? validation = null)
        {
            DeleteContactHelper(contactId: contactId, output: output,
                                validation: validation);
        }

        /// <summary>
        /// This action is used for contact deleting. 
        /// </summary>
        /// <param name="contactType">type of contact (Email, ICQ, etc.)</param>
        /// <param name="account">account information depending on contact type</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void DeleteContact(string account,ContactType contactType, 
            OutputType? output = null, Validation? validation = null)
        {
            DeleteContactHelper(contactType: contactType, account: account, output: output,
                                validation: validation);
        }

        /// <summary>
        /// Helper for DeleteContact action. 
        /// </summary>
        /// <param name="contactId">id of the contact to delete</param>
        /// <param name="contactType">type of contact (Email, ICQ, etc.)</param>
        /// <param name="account">account information depending on contact type</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        private void DeleteContactHelper(int? contactId=null, ContactType? contactType=null, string account=null, OutputType? output=null,
                                         Validation? validation=null)
        {
            var parameters = new Dictionary<string, object>();
            AddIfNotNull(parameters, Params.contactId, contactId);
            if (contactType!=null)
                AddIfNotNull(parameters, Params.contactType, (int)contactType);
            AddIfNotNull(parameters, Params.account, account);
            if (0 == parameters.Count)
                throw new Exceptions.ParamsNotSpecifiedOrHasNullValuesException();

            RestResponse response = MakePostRequest(ContactAction.deleteContact, parameters, output: output,
                                           validation: validation);
            Helper.CheckStatus(response, GetOutput(output));
        }

        /// <summary>
        /// This action is used to confirm the specified contact. 
        /// </summary>
        /// <param name="contactId">id of the contact to confirm</param>
        /// <param name="confirmationKey">confirmation key of the contact</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void ConfirmContact(int contactId, string confirmationKey, OutputType? output = null,
                                   Validation? validation = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.contactId, contactId);
            parameters.Add(Params.confirmationKey, confirmationKey);
            RestResponse response = MakePostRequest(ContactAction.confirmContact, parameters, output: output,
                                           validation: validation);
            Helper.CheckStatus(response, GetOutput(output));
        }

        /// <summary>
        /// This action is used for contact activating. 
        /// </summary>
        /// <param name="contactId">id of the contact to activate</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void ActivateContact(int contactId, OutputType? output = null,
                                    Validation? validation = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.contactId, contactId);
            RestResponse response = MakePostRequest(ContactAction.contactActivate, parameters, output: output,
                                           validation: validation);
            Helper.CheckStatus(response, GetOutput(output));
        }

        /// <summary>
        /// This action is used for contact deactivating. 
        /// </summary>
        /// <param name="contactId">id of the contact to deactivate</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void DeActivateContact(int contactId, OutputType? output = null,
                                      Validation? validation = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.contactId, contactId);
            RestResponse response = MakePostRequest(ContactAction.contactDeactivate, parameters, output: output,
                                           validation: validation);
            Helper.CheckStatus(response, GetOutput(output));
        }
    }
}