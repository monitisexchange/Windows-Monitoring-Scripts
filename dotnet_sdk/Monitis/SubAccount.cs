using System;
using System.Collections.Generic;
using RestSharp;

namespace Monitis
{
    /// <summary>
    /// TODO: count of subaccount cannot be more than 5! Fix?
    /// </summary>
    public class SubAccount : APIObject
    {
        private enum SubAccountAction
        {
            subAccounts,
            subAccountPages,
            addSubAccount,
            deleteSubAccount,
            addPagesToSubAccount,
            deletePagesFromSubAccount
        }

        #region Constructors

        public SubAccount()
        {
        }

        public SubAccount(string apiKey, string secretKey)
            : base(apiKey, secretKey)
        {

        }

        public SubAccount(string apiKey, string secretKey, string apiUrl)
            : base(apiKey, secretKey, apiUrl)
        {

        }

        public SubAccount(Authentication authentication)
            : base(authentication)
        {

        }

        #endregion

        /// <summary>
        /// This action is used to get user's all sub accounts. 
        /// </summary>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <returns>all subaccounts</returns>
        public virtual Structures.SubAccount[] GetSubAccounts(OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            RestResponse response = MakeGetRequest(SubAccountAction.subAccounts, output: outputType);
            Structures.SubAccount[] subAccounts = Helper.DeserializeObject<Structures.SubAccount[]>(response, outputType,
                                                                                                    Params.subaccounts);
            return subAccounts;
        }

        /// <summary>
        /// This action is used to get all pages for user's all sub accounts. 
        /// </summary>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <returns>subaccount pages</returns>
        public virtual Structures.SubAccountPage[] GetSubAccountPages(OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            RestResponse response = MakeGetRequest(SubAccountAction.subAccountPages, output: outputType);
            string content = response.Content;
            //TODO:fix response from server (add 'subaccount' tag for each item)
            if (OutputType.XML == outputType)
            {
                content = content.Replace("<id>", "<" + Params.subaccountpage + "><id>");
                content = content.Replace("</pages>", "</pages>"+"</" + Params.subaccountpage + ">");
            }
          
            Structures.SubAccountPage[] pages = Helper.DeserializeObject<Structures.SubAccountPage[]>(content,
                                                                                                      outputType,
                                                                                                      Params.subaccounts);
            return pages;
        }

        /// <summary>
        /// This action is used to add a new sub account. 
        /// </summary>
        /// <param name="firstName">first name of the sub account owner</param>
        /// <param name="lastName">last name of the sub account owner</param>
        /// <param name="email">email address which will be used as username for the sub account</param>
        /// <param name="password">password of the sub account</param>
        /// <param name="group">group of the sub account</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        /// <returns>id of added subaccount</returns>
        public int AddSubAccount(string firstName, string lastName, string email, string password, string group,
                                 OutputType? output = null, Validation? validation = null)
        {
            int userId;
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.firstName, firstName);
            parameters.Add(Params.lastName, lastName);
            parameters.Add(Params.email, email);
            parameters.Add(Params.password, password);
            parameters.Add(Params.group, group);
            RestResponse response = MakePostRequest(SubAccountAction.addSubAccount, parameters, output: outputType,
                                           validation: validation);
            Helper.CheckStatus(response, outputType);
            userId = Convert.ToInt32(Helper.GetValueOfKey(response, Params.userId, outputType));
            return userId;
        }

        /// <summary>
        /// This action is used to delete the specified sub account. 
        /// </summary>
        /// <param name="userId">id of the sub account to delete</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void DeleteSubAccount(int userId,
                                     OutputType? output = null, Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.userId, userId);
            RestResponse response = MakePostRequest(SubAccountAction.deleteSubAccount, parameters, output: outputType,
                                           validation: validation);
            Helper.CheckStatus(response, outputType);
        }

        /// <summary>
        /// This action is used to add new pages for the specified sub account. 
        /// </summary>
        /// <param name="userId">user id of the sub account to add pages to</param>
        /// <param name="pageNames">names of your pages you want to share with your sub account </param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void AddPagesToSubAccount(int userId, string[] pageNames,
                                         OutputType? output = null, Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.userId, userId);
            parameters.Add(Params.pageNames, string.Join(Helper.DataSeparator, pageNames));
            RestResponse response = MakePostRequest(SubAccountAction.addPagesToSubAccount, parameters, output: outputType,
                                           validation: validation);
            Helper.CheckStatus(response, outputType);
        }

        /// <summary>
        /// This action is used to delete the specified pages from specified sub account. 
        /// </summary>
        /// <param name="userId">id of the sub account to delete pages from</param>
        /// <param name="pageNames">names of pages to remove</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void DeletePagesFromSubAccount(int userId, string[] pageNames,
                                              OutputType? output = null, Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.userId, userId);
            parameters.Add(Params.pageNames, string.Join(Helper.DataSeparator, pageNames));
            RestResponse response = MakePostRequest(SubAccountAction.deletePagesFromSubAccount, parameters,
                                                    output: output, validation: validation);
            Helper.CheckStatus(response, outputType);
        }
    }
}