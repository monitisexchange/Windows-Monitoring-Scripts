using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Monitis.Tests
{
    [TestFixture]
    public class SubAccountTests
    {
        private Authentication authentication = null;
        private SubAccount subAccount = null;

        [TestFixtureSetUp]
        public void Setup()
        {
            authentication = new Authentication(apiKey: MonitisAccountInformation.ApiKey,
                                                secretKey: MonitisAccountInformation.SekretKey);
            subAccount=new SubAccount();
            subAccount.SetAuthenticationParams(authentication);

            //Add 1 subaccount for test, if no subaccount exist
            var subAccounts = subAccount.GetSubAccounts();
            int countOfSubAccounts = subAccounts.Length;
            if (countOfSubAccounts == 0)
                AddSubAccount(OutputType.JSON);
                //Cannot be more than 5 subaccounts
            else if (countOfSubAccounts==5)
            {
                subAccount.DeleteSubAccount(subAccounts.Last().id);
            }
        }

        #region GetSubAccounts

        [Test]
        public void GetSubAccounts_ShowCountOfSubAccounts_Json()
        {
            var output = OutputType.JSON;
             GetSubAccounts_ShowCountOfSubAccounts(output);
        }
        [Test]
        public void GetSubAccounts_ShowCountOfSubAccounts_Xml()
        {
            var output = OutputType.XML;
            GetSubAccounts_ShowCountOfSubAccounts(output);
        }

        private void GetSubAccounts_ShowCountOfSubAccounts(OutputType output)
        {
            var subAccounts = subAccount.GetSubAccounts(output);
            Assert.Pass("Count of subaccounts: " + subAccounts.Length);
        }

        #endregion

        #region GetSubAccountPages

        [Test]
        public void GetSubAccountPages_AddSubAccountCheckCountOfPagesMoreThanOneDeleteSubAccount_Json()
        {
            var output = OutputType.JSON;
            GetSubAccountPages_AddSubAccountCheckCountOfPagesMoreThanOneDeleteSubAccount(output);
        }
        [Test]
        public void GetSubAccountPages_AddSubAccountCheckCountOfPagesMoreThanOneDeleteSubAccount_Xml()
        {
            var output = OutputType.XML;
            GetSubAccountPages_AddSubAccountCheckCountOfPagesMoreThanOneDeleteSubAccount(output);
        }

        private void GetSubAccountPages_AddSubAccountCheckCountOfPagesMoreThanOneDeleteSubAccount(OutputType output)
        {
            int idSubAccountToDelete = AddSubAccount(output);
            subAccount.AddPagesToSubAccount(idSubAccountToDelete, MonitisAccountInformation.PagesInYourAccount);
            var subAccountsPages = subAccount.GetSubAccountPages(output);
            if (subAccountsPages.First(p => p.id == idSubAccountToDelete).pages.Length == MonitisAccountInformation.PagesInYourAccount.Length)
            {
                subAccount.DeleteSubAccount(idSubAccountToDelete);
                Assert.Pass("Count of pages correct!");
               
            }
            else
            {
                subAccount.DeleteSubAccount(idSubAccountToDelete);
                Assert.Fail("Count of pages incorrect!");
            }
          
        }

        #endregion

        #region AddSubAccount

        [Test]
        public void AddSubAccount_AddAndDeleteSubaccount_Json()
        {
            var output = OutputType.JSON;
            AddSubAccountAndDelete(output);
        }

        [Test]
        public void AddSubAccount_AddAndDeleteSubaccount_Xml()
        {
            var output = OutputType.XML;
            AddSubAccountAndDelete(output);
        }

        private void AddSubAccountAndDelete(OutputType output)
        {
            int idSubAccount = AddSubAccount(output);
            subAccount.DeleteSubAccount(idSubAccount);
            Assert.Pass("Subaccount with id" + idSubAccount + " added and deleted successfully!");
        }

        private int AddSubAccount(OutputType output)
        {
            string firstName = Common.GenerateRandomString(8);
            string lastName = Common.GenerateRandomString(8);
            string email = Common.GenerateRandomString(15) + "@mailforspam.com";
            string password = "123";
            string group = "testSubAccountGroup";
            int idSubAccount = subAccount.AddSubAccount(firstName, lastName, email, password, group, output);
            return idSubAccount;
        }

        #endregion

        #region DeleteSubAccount

        [Test]
        public void DeleteSubAccount_AddSubAccountAndDeleteAndCheckCountOfSubaccounts_UsingAddSubAccountUsingGetSubAccounts_Json()
        {
            var output = OutputType.JSON;
            DeleteSubAccount_AddSubAccountAndDeleteAndCheckCountOfSubaccounts_UsingAddSubAccountUsingGetSubAccounts(
                output);
        }

        [Test]
        public void DeleteSubAccount_AddSubAccountAndDeleteAndCheckCountOfSubaccounts_UsingAddSubAccountUsingGetSubAccounts_Xml()
        {
            var output = OutputType.XML;
            DeleteSubAccount_AddSubAccountAndDeleteAndCheckCountOfSubaccounts_UsingAddSubAccountUsingGetSubAccounts(
                output);
        }

        private void DeleteSubAccount_AddSubAccountAndDeleteAndCheckCountOfSubaccounts_UsingAddSubAccountUsingGetSubAccounts(OutputType output)
        {
            int idSubAccount = AddSubAccount(output);
            DeleteSubAccount(output, idSubAccount);

        }

        private void DeleteSubAccount(OutputType output, int idSubAccountToDelete)
        {
            var subAccounts = subAccount.GetSubAccounts(output);
            int countBegin= subAccounts.Length;
            subAccount.DeleteSubAccount(idSubAccountToDelete, output);
            int countAfterDelete = subAccount.GetSubAccounts(output).Length;
            if (countBegin - 1 == countAfterDelete)
                Assert.Pass("Subaccount deleted");
            else
            {
                Assert.Fail("Fail: count of subAccount has not decreased by one. Count before delete:" + countBegin + " count after delete: " + countAfterDelete);
            }
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Invalid subaccount userId")]
        public void DeleteSubAccount_IncorrectSubAccountId_ThrowsException_Xml()
        {
            var output = OutputType.XML;
            subAccount.DeleteSubAccount(-1,output);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Invalid subaccount userId")]
        public void DeleteSubAccount_IncorrectSubAccountId_ThrowsException_Json()
        {
            var output = OutputType.JSON;
            subAccount.DeleteSubAccount(-1,output);
        }

        #endregion

        #region AddPagesToSubAccount

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Invalid page: invalid page")]
        public void AddPagesToSubAccount_InvalidPage_ThrowsException_Xml()
        {
            var output = OutputType.XML;
            var subAccounts = subAccount.GetSubAccounts(output);
            subAccount.AddPagesToSubAccount(subAccounts.First().id, new string[] {"invalid page"});
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Invalid page: invalid page")]
        public void AddPagesToSubAccount_InvalidPage_ThrowsException_Json()
        {
            var output = OutputType.JSON;
            var subAccounts = subAccount.GetSubAccounts(output);
            subAccount.AddPagesToSubAccount(subAccounts.First().id, new string[] { "invalid page" });
        }

        [Test]
        public void AddPagesToSubAccount_AccountPagesAddedToFirstSubAccount_Xml()
        {
            var output = OutputType.XML;
            AddPagesToSubAccount_AccountPagesAddedToFirstSubAccount(output);
        }

        [Test]
        public void AddPagesToSubAccount_AccountPagesAddedToFirstSubAccount_Json()
        {
            var output = OutputType.JSON;
            AddPagesToSubAccount_AccountPagesAddedToFirstSubAccount(output);
        }

        private int AddPagesToSubAccount_AccountPagesAddedToFirstSubAccount(OutputType output)
        {
            var subAccounts = subAccount.GetSubAccounts(output);
            //if response ok, no exception, pages added
            int idFirstSubAccount = subAccounts.First().id;
            subAccount.AddPagesToSubAccount(idFirstSubAccount, MonitisAccountInformation.PagesInYourAccount);
            return idFirstSubAccount;
        }

        #endregion

        #region DeletePagesFromSubAccount

        [Test]
        public void DeletePagesFromSubAccount_AddAndDeletePagesFromFirstSubAccount_Xml()
        {
            var output = OutputType.XML;
            DeletePagesFromSubAccount_AddAndDeletePagesFromFirstSubAccount(output);
        }

        [Test]
        public void DeletePagesFromSubAccount_AddAndDeletePagesFromFirstSubAccount_Json()
        {
            var output = OutputType.JSON;
            DeletePagesFromSubAccount_AddAndDeletePagesFromFirstSubAccount(output);
        }

        private void DeletePagesFromSubAccount_AddAndDeletePagesFromFirstSubAccount(OutputType output)
        {
            int idSubAccount= AddPagesToSubAccount_AccountPagesAddedToFirstSubAccount(output);
            subAccount.DeletePagesFromSubAccount(idSubAccount,
                                                                MonitisAccountInformation.PagesInYourAccount);
        }

        #endregion
    }
}
