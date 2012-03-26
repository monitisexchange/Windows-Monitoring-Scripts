using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Monitis.Tests
{
    [TestFixture]
    public class ContactTests
    {
        private Authentication authentication = null;
        private Contact contact = null;

        [TestFixtureSetUp]
        public void Setup()
        {
            authentication = new Authentication(apiKey: MonitisAccountInformation.ApiKey,
                                                secretKey: MonitisAccountInformation.SekretKey);

            contact = new Contact();
            contact.SetAuthenticationParams(authentication);
        }

        #region GetContacts

        [Test]
        public void GetContacts_MoreThanOneContact_Json_ReturnsTrue()
        {
            OutputType outputType = OutputType.JSON;
            Assert.IsTrue(contact.GetContacts(outputType).Length > 0, "Count of contacts = 0");
        }

        [Test]
        public void GetContacts_MoreThanOneContact_Xml_ReturnsTrue()
        {
            OutputType outputType = OutputType.XML;
            Assert.IsTrue(contact.GetContacts(outputType).Length > 0, "Count of contacts = 0");
        }

        #endregion

        #region GetContactGroups

        [Test]
        public void GetContactGroups_Xml()
        {
            OutputType outputType = OutputType.XML;
            contact.GetContactGroups(outputType);
        }

        [Test]
        public void GetContactGroups_Json()
        {
            OutputType outputType = OutputType.JSON;
            contact.GetContactGroups(outputType);
        }

        #endregion

        //TODO: add alerts and check
        #region GetRecentAlerts

        [Test]
        public void GetRecentAlerts_IsNotNull_Json_ReturnsTrue()
        {
            OutputType outputType = OutputType.JSON;
            GetRecentAlerts_IsNotNull(outputType);
        }

        [Test]
        public void GetRecentAlerts_IsNotNull_Xml_ReturnsTrue()
        {
            OutputType outputType = OutputType.XML;
            GetRecentAlerts_IsNotNull(outputType);
        }

        private void GetRecentAlerts_IsNotNull(OutputType outputType)
        {
            var alerts = contact.GetRecentAlerts(limit:-1000, output: outputType);
            Assert.IsNotNull(alerts);
        }

        #endregion

        #region AddContact

        [Test]
        [ExpectedException(typeof (Exception), ExpectedMessage = "Contact already exists")]
        public void AddContact_ContactAlreadyExist_Xml_ThrowsException()
        {
            OutputType outputType = OutputType.XML;
            var contactFirst = contact.GetContacts()[0];
            contact.AddContact(
                MonitisAccountInformation.FirstName,
                MonitisAccountInformation.LastName,
                contactFirst.contactAccount, contactFirst.contactType,
                output: outputType);
        }


        [Test]
        [ExpectedException(typeof (Exception), ExpectedMessage = "Contact already exists")]
        public void AddContact_ContactAlreadyExist_Json_ThrowsException()
        {
            OutputType outputType = OutputType.JSON;
            var contactFirst = contact.GetContacts()[0];
            contact.AddContact(
                MonitisAccountInformation.FirstName,
                MonitisAccountInformation.LastName,
                contactFirst.contactAccount, contactFirst.contactType,
                output: outputType);
        }

        [Test]
        public void AddContact_ContactAddedWithRandomMailAndDeleted_Xml_UsingGetContactsUsingDeleteContact_ReturnsTrue()
        {
            OutputType outputType = OutputType.XML;
            string login = Common.GenerateRandomString(15) + "@mailforspam.com";
            contact.AddContact(
                "AddRandomContactName",
                "AddRandomContactSurname",
                login, Contact.ContactType.Email,
                output: outputType);
            //clear contact
            var contactLast = contact.GetContacts().Last();
            contact.DeleteContact(contactId: contactLast.contactId);
            Assert.Pass("Contact with login " + contactLast.contactAccount + " added and deleted successfully");
        }

        [Test]
        public void AddContact_ContactAddedWithRandomMailAndDeleted_Json_UsingGetContactsUsingDeleteContact_ReturnsTrue()
        {
            OutputType outputType = OutputType.JSON;
            string login = Common.GenerateRandomString(15) + "@mailforspam.com";
            contact.AddContact(
                "AddRandomContactName",
                "AddRandomContactSurname",
                login, Contact.ContactType.Email,
                output: outputType);
            //clear contact
            var contactLast = contact.GetContacts().Last();
            contact.DeleteContact(contactId: contactLast.contactId);
            Assert.Pass("Contact with login " + contactLast.contactAccount + " added and deleted successfully");
        }

        #endregion

        #region EditContact

        [Test]
        public void EditContact_ContactAddedEditedAndDeleted_Json_UsingGetContactsUsingAddContactUsingDeleteContact()
        {
            OutputType outputType = OutputType.JSON;
            EditContact(outputType);
        }

        [Test]
        public void EditContact_ContactAddedEditedAndDeleted_Xml_UsingGetContactsUsingAddContactUsingDeleteContact()
        {
            OutputType outputType = OutputType.XML;
            EditContact(outputType);
        }

        private void EditContact(OutputType outputType)
        {
            //Add new contact for edit
            string login = Common.GenerateRandomString(15) + "@mailforspam.com";
            var addedContactInfo = contact.AddContact(
                "AddRandomContactName",
                "AddRandomContactSurname",
                login, Contact.ContactType.Email,
                output: outputType);

            //get new contact from service
            var contactAdded = contact.GetContacts().FirstOrDefault(c => c.contactId == addedContactInfo.Value);

            //prepare params to edit contact
            Contact.ContactType newContactType = Contact.ContactType.ICQ;
            string newFirstname = "testFirstName";
            string newLastname = "testlastName";
            string newAccount = Common.GenerateRandomString(15) + "@mailforspam.com";
            string newCountry = "United States";
            int timeZone = 240;

            //edit contact
           var key=  contact.EditContact(contactAdded.contactId,
                                contactType: newContactType,
                                firstName: newFirstname,
                                lastName: newLastname,
                                account: newAccount,
                                country: newCountry,
                                timezone: timeZone,
                                output: outputType);

            //check on success edit
            var contactFirstChanged = contact.GetContacts()
                .FirstOrDefault(c => c.contactId == addedContactInfo.Value);
            if (contactFirstChanged.contactType == newContactType &&
                contactFirstChanged.name == newFirstname + " " + newLastname &&
                contactFirstChanged.contactAccount == newAccount &&
                contactFirstChanged.country == newCountry &&
                contactFirstChanged.timezone == timeZone)
            {
                contact.DeleteContact(contactAdded.contactId);
                Assert.Pass("Contact with login " + contactAdded.contactAccount + " contact succesfully created, edited and deleted");
            }
            else
            {
                contact.DeleteContact(contactAdded.contactId);
                Assert.Fail("Contact with login " + contactAdded.contactAccount + " contact NOT succesfully edited, but was deleted");
            }
        }

        #endregion

        #region DeleteContact

        [Test]
        public void DeleteContact_AddContactAndDeletesContactBycontactId_Xml_UsingGetContactsUsingAddContact()
        {
            OutputType outputType = OutputType.XML;
            DeleteContactById(outputType);
        }

        [Test]
        public void DeleteContact_AddContactAndDeletesContactBycontactId_Json_UsingGetContactsUsingAddContact()
        {
            OutputType outputType = OutputType.JSON;
            DeleteContactById(outputType);
        }

        private void DeleteContactById(OutputType output)
        {
            //Add new contact for test
            string login = Common.GenerateRandomString(15) + "@mailforspam.com";
            var addedContactInfo = contact.AddContact(
                "AddRandomContactName",
                "AddRandomContactSurname",
                login, Contact.ContactType.Email,
                output: output);
            int idContactBeforeDelete = addedContactInfo.Value;
            contact.DeleteContact(contactId: addedContactInfo.Value, output: output);
  
            if (!contact.GetContacts().Any(c => c.contactId == idContactBeforeDelete))
            {
                Assert.Pass("Contact with id " + idContactBeforeDelete + " added and deleted succesfuly");
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void DeleteContact_AddContactAndDeletesContactByLoginAndContactType_Json_UsingGetContactsUsingAddContact()
        {
            OutputType outputType = OutputType.JSON;
            DeleteContactByLoginAndContactType(outputType);
        }

        [Test]
        public void DeleteContact_AddContactAndDeletesContactByLoginAndContactType_Xml_UsingGetContactsUsingAddContact()
        {
            OutputType outputType = OutputType.XML;
            DeleteContactByLoginAndContactType(outputType);
        }

        private void DeleteContactByLoginAndContactType(OutputType output)
        {
            //Add new contact for test
            string login = Common.GenerateRandomString(15) + "@mailforspam.com";
            var currentContactType = Contact.ContactType.Email;
            var addedContactInfo = contact.AddContact(
                "AddRandomContactName",
                "AddRandomContactSurname",
                login,currentContactType,
                output: output);
            contact.DeleteContact(account: login,contactType:currentContactType, output: output);

            if (!contact.GetContacts().Any(c => c.contactAccount == login&&c.contactType==currentContactType))
            {
                Assert.Pass("Contact with login " + login + " and contact type " + currentContactType+ " added and deleted succesfuly");
            }
            else
            {
                Assert.Fail("Contact with login " + login + " and contact type " + currentContactType+ " exists, not deleted!");
            }
        }

        
        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Invalid contact details")]
        public void DeleteContact_DeleteContactWithEmptyLogin_Xml_ThrowsException()
        {
            OutputType outputType = OutputType.XML;
            contact.DeleteContact("", Contact.ContactType.URL, outputType);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Invalid contact details")]
        public void DeleteContact_DeleteContactWithEmptyLogin_Json_ThrowsException()
        {
            OutputType outputType = OutputType.JSON;
            contact.DeleteContact("", Contact.ContactType.URL, outputType);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Neither 'contactId' nor the pair of 'contactType' and 'account' were specified.")]
        public void DeleteContact_DeleteContactWithNullLogin_Xml_ThrowsException()
        {
            OutputType outputType = OutputType.XML;
            contact.DeleteContact(null, Contact.ContactType.URL, outputType);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Neither 'contactId' nor the pair of 'contactType' and 'account' were specified.")]
        public void DeleteContact_DeleteContactWithNullLogin_Json_ThrowsException()
        {
            OutputType outputType = OutputType.JSON;
            contact.DeleteContact(null, Contact.ContactType.URL, outputType);
        }

        [Test]
        [ExpectedException(typeof (Exception), ExpectedMessage = "Invalid contact id")]
        public void DeleteContact_DeleteNotExistContact_Json_UsingGetContactsUsingAddContact_ThrowsException()
        {
            OutputType outputType = OutputType.JSON;
            contact.DeleteContact(contactId: -1, output: outputType);
        }

        [Test]
        [ExpectedException(typeof (Exception), ExpectedMessage = "Invalid contact id")]
        public void DeleteContact_DeleteNotExistContact_Xml_UsingGetContactsUsingAddContact_ThrowsException()
        {
            OutputType outputType = OutputType.XML;
            contact.DeleteContact(contactId: -1, output: outputType);
        }

        #endregion

        #region ConfirmContact

        [Test]
        public void ConfirmContact_AddContactConfirmAndDelete_Xml_UsingAddContactUsingDeleteContact()
        {
            OutputType outputType = OutputType.XML;
            ConfirmContact(outputType);
        }

        [Test]
        public void ConfirmContact_AddContactConfirmAndDelete_Json_UsingAddContactUsingDeleteContact()
        {
            OutputType outputType = OutputType.JSON;
            ConfirmContact(outputType);
        }

        private void ConfirmContact(OutputType output)
        {
            //Add new contact for test
            string login = Common.GenerateRandomString(15) + "@mailforspam.com";
            var addedContactInfo = contact.AddContact(
                "AddRandomContactName",
                "AddRandomContactSurname",
                login, Contact.ContactType.Email,
                output: output);

            contact.ConfirmContact(contactId: addedContactInfo.Value,
                                   confirmationKey: addedContactInfo.Key,
                                   output: output);

            contact.DeleteContact(addedContactInfo.Value);
        }

        private void ConfirmContactWithIncorrectCode(OutputType output, string confirmIncorrectCode = null)
        {
            var contactFirst = contact.GetContacts(output: output).First();

            contact.ConfirmContact(contactId: contactFirst.contactId,
                                   confirmationKey: confirmIncorrectCode,
                                   output: output);
        }

        [Test]
        [ExpectedException(typeof (Exception), ExpectedMessage = "Verification code is not valid.")]
        public void ConfirmContact_ConfirmWithIncorrectCode_Json_UsingGetContacts_ThrowsException()
        {
            OutputType outputType = OutputType.JSON;
            ConfirmContactWithIncorrectCode(outputType, "incorrectConfirmCode");
        }

        [Test]
        [ExpectedException(typeof (Exception), ExpectedMessage = "Verification code is not valid.")]
        public void ConfirmContact_ConfirmWithIncorrectCode_Xml_UsingGetContacts_ThrowsException()
        {
            OutputType outputType = OutputType.XML;
            ConfirmContactWithIncorrectCode(outputType, "incorrectConfirmCode");
        }

        #endregion

        #region ActivateContact

        [Test]
        public void ActivateContact_ActivatesFirstContact_Json_UsingGetContactsUsingDeactivateContact()
        {
            OutputType outputType = OutputType.JSON;
            ActivateContact(outputType);
        }

        [Test]
        public void ActivateContact_ActivatesFirstContact_Xml_UsingGetContactsUsingDeactivateContact()
        {
            OutputType outputType = OutputType.XML;
            ActivateContact(outputType);
        }

        public void ActivateContact(OutputType outputType)
        {
            var contactFirst = contact.GetContacts(output: outputType).First();
            contact.ActivateContact(contactFirst.contactId, outputType);
            if (1 == contact.GetContacts(output: outputType).First().activeFlag)
            {
                if (contactFirst.confirmationFlag == 0)
                    contact.DeActivateContact(contactFirst.contactId);
                Assert.Pass("Contact with id "+contactFirst.contactId + " activated and restored to previous state");
            }
            else
            {
                Assert.Fail("Contact with id "+contactFirst.contactId + " was not activated");
            }
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Invalid contactId")]
        public void ActivateContact_IncorrectContactId_Xml_ThrowsException()
        {
            OutputType outputType = OutputType.XML;
            contact.ActivateContact(-1, outputType);
        }
       
        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Invalid contactId")]
        public void ActivateContact_IncorrectContactId_Json_ThrowsException()
        {
            OutputType outputType = OutputType.JSON;
            contact.ActivateContact(-1, outputType);
        }

        #endregion

        #region DeActivateContact

        [Test]
        public void DeActivateContact_DeactivatesFirstContactAndRestoresState_Xml_UsingGetContactsUsingActivateContact()
        {
            OutputType outputType = OutputType.XML;
            Deactivate(outputType);
        }

        [Test]
        public void DeActivateContact_DeactivatesFirstContactAndRestoresState_Json_UsingGetContactsUsingActivateContact()
        {
            OutputType outputType = OutputType.JSON;
            Deactivate(outputType);
        }

        public void Deactivate(OutputType outputType)
        {
            var contactFirst = contact.GetContacts(output: outputType).First();
            contact.DeActivateContact(contactFirst.contactId, outputType);
            if (0 == contact.GetContacts(output: outputType).First().activeFlag)
            {
                if (contactFirst.confirmationFlag == 1)
                    contact.ActivateContact(contactFirst.contactId);
                Assert.Pass("Contact with id "+contactFirst.contactId + " deactivated and restored to previous state");
            }
            else
            {
                Assert.Fail("Contact with id "+contactFirst.contactId + " was not deactivated");
            }
        }

        #endregion
    }
}
