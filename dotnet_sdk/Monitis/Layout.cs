using System;
using System.Collections.Generic;
using RestSharp;

namespace Monitis
{
    public class Layout : APIObject
    {
        private enum LayoutAction
        {
            pages,
            pageModules,
            addPage,
            addPageModule,
            deletePage,
            deletePageModule,
        }

        /// <summary>
        /// possible values of module's name
        /// </summary>
        public enum ModulName
        {
            External,
            Process,
            Drive,
            Memory,
            InternalHTTP,
            InternalPing,
            LoadAverage,
            CPU,
            Transaction,
            Fullpageload,
            VisitorsTracking,
            CustomMonitor
        }

        #region Constructors

        public Layout()
            : base()
        {
        }


        public Layout(string apiKey, string secretKey)
            : base(apiKey, secretKey)
        {

        }

        public Layout(string apiKey, string secretKey, string apiUrl)
            : base(apiKey, secretKey, apiUrl)
        {

        }

        public Layout(Authentication authentication)
            : base(authentication)
        {

        }

        #endregion

        /// <summary>
        /// This action is used to get user's all pages. 
        /// </summary>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <returns>pages of user</returns>
        public Structures.Page[] GetPages(OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            RestResponse response = MakeGetRequest(LayoutAction.pages, output: outputType);
            Structures.Page[] pages = Helper.DeserializeObject<Structures.Page[]>(response, outputType, Params.pages);
            return pages;
        }

        /// <summary>
        /// This action is used to get all modules of the specified page.
        /// </summary>
        /// <param name="pageName">name of the page to get modules for</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <returns>modules of specified page. If name of page incorrect, there is will be 0 modules</returns>
        public Structures.PageModule[] GetPageModules(string pageName, OutputType? output = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            AddIfNotNull(parameters, Params.pageName, pageName);
            RestResponse response = MakeGetRequest(LayoutAction.pageModules, parameters, outputType);
            Structures.PageModule[] pageModules =
                Helper.DeserializeObject<Structures.PageModule[]>(response, outputType, Params.pageModules);
            return pageModules;
        }

        /// <summary>
        /// This action is used to add a new page to user's dash board. 
        /// </summary>
        /// <param name="title">title of the page</param>
        /// <param name="columnCount">count of columns on the page</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        /// <returns>id of added page</returns>
        public int AddPage(string title, int? columnCount = null, OutputType? output = null,
                           Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            int pageId;
            var parameters = new Dictionary<string, object>();
            AddIfNotNull(parameters, Params.title, title);
            AddIfNotNull(parameters, Params.columnCount, columnCount);
            RestResponse response = MakePostRequest(LayoutAction.addPage, parameters, output: outputType,
                                                    validation: validation);
            Helper.CheckStatus(response, outputType);
            var result = Helper.GetValueOfKey(response, Params.pageId, outputType);
            Int32.TryParse(result, out pageId);
            return pageId;
        }

        /// <summary>
        /// This action is used for deleting the specified page from user's dash board. 
        /// </summary>
        /// <param name="pageId">id of the page to delete</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void DeletePage(int pageId, OutputType? output = null, Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.pageId, pageId);
            RestResponse response = MakePostRequest(LayoutAction.deletePage, parameters, output: outputType,
                                                    validation: validation);
            Helper.CheckStatus(response, outputType);
        }

        /// <summary>
        /// This action is used for deleting the specified module from the page. 
        /// </summary>
        /// <param name="pageModuleId">id of the page module to delete</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        public void DeletePageModule(int pageModuleId, OutputType? output = null, Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.pageModuleId, pageModuleId);
            var response= MakePostRequest(LayoutAction.deletePageModule, parameters, output: output, validation: validation);
            Helper.CheckStatus(response, output: outputType);
        }

        /// <summary>
        /// This action is used to add a module to the specified page. 
        /// </summary>
        /// <param name="moduleName">name of the module to add</param>
        /// <param name="pageId">id of the page to add module to</param>
        /// <param name="column">number of the column to add module to</param>
        /// <param name="row">number of the row to add module to</param>
        /// <param name="dataModuleId">id of the test to add view for
        /// for example if moduleName=External dataModuleId should be id of the External monitor, which results you want to be shown on the page</param>
        /// <param name="height">height of the module in pixels (default value is 200)</param>
        /// <param name="output">Type of output - XML or JSON</param>
        /// <param name="validation">HMACSHA1 for checksum validation or token for authToken validation</param>
        /// <returns>pageModuleId</returns>
        public int AddPageModule(ModulName moduleName, int pageId, int column, int row, int dataModuleId,
                                 int? height = null, OutputType? output = null, Validation? validation = null)
        {
            OutputType outputType = GetOutput(output);
            var parameters = new Dictionary<string, object>();
            parameters.Add(Params.moduleName, moduleName);
            parameters.Add(Params.pageId, pageId);
            parameters.Add(Params.row, row);
            parameters.Add(Params.column, column);
            AddIfNotNull(parameters, Params.height, height);
            parameters.Add(Params.dataModuleId, dataModuleId);
            RestResponse response = MakePostRequest(LayoutAction.addPageModule, parameters, output: outputType,
                                                    validation: validation);
            Helper.CheckStatus(response, outputType);
            int pageModuleId;
            string key = Helper.GetValueOfKey(response, Params.pageModuleId, outputType);
            Int32.TryParse(key, out pageModuleId);
            return pageModuleId;
        }
    }
}