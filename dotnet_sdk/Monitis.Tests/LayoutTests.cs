using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Monitis.Tests
{
    [TestFixture]
    public class LayoutTests
    {
        private Authentication authentication = null;
        private Layout layout = null;

        [TestFixtureSetUp]
        public void Setup()
        {
            authentication = new Authentication(apiKey: MonitisAccountInformation.ApiKey,
                                                secretKey: MonitisAccountInformation.SekretKey);

            layout = new Layout();
            layout.SetAuthenticationParams(authentication);
        }


        #region GetPages

        [Test]
        public void GetPages_IsNotNull_Json()
        {
            OutputType outputType = OutputType.JSON;
            GetPages_IsNotNull(outputType);
        }

        [Test]
        public void GetPages_IsNotNull_Xml()
        {
            OutputType outputType = OutputType.XML;
            GetPages_IsNotNull(outputType);
        }

        private void GetPages_IsNotNull(OutputType output)
        {
            var result = layout.GetPages(output);
            Assert.IsNotNull(result);
        }

        #endregion

        #region GetPageModules

        [Test]
        public void GetPageModules_GetModulesOFirstPage_UsingGetPages_Json()
        {
            OutputType outputType = OutputType.JSON;
            GetPageModules_GetModulesOFirstPage_UsingGetPages(outputType);
        }

        [Test]
        public void GetPageModules_GetModulesOFirstPage_UsingGetPages_Xml()
        {
            OutputType outputType = OutputType.XML;
            GetPageModules_GetModulesOFirstPage_UsingGetPages(outputType);
        }

        private Structures.PageModule[] GetPageModules_GetModulesOFirstPage_UsingGetPages(OutputType output)
        {
            string firstPageTitle = layout.GetPages(output).First().title;
            var response = layout.GetPageModules(firstPageTitle, output);
            Assert.IsNotNull(response);
            return response;
        }

        #endregion

        #region AddPage

        [Test]
        public void AddPage_AddPageAndDelete_Json()
        {
            OutputType outputType = OutputType.JSON;
            AddPageAndDelete(outputType);
        }

        [Test]
        public void AddPage_AddPageAndDelete_Xml()
        {
            OutputType outputType = OutputType.XML;
            AddPageAndDelete(outputType);
        }

        private void AddPageAndDelete(OutputType output)
        {
            int pageId = layout.AddPage("Test api page" + Common.GenerateRandomString(5), 2, output);
            layout.DeletePage(pageId, output);
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Page with that name already exists")]
        public void AddPage_AddFirstPageThatAlreadyExists_Throws_Exception_Json()
        {
            OutputType outputType = OutputType.JSON;
            AddPage_AddFirstPageThatlreadyExists(outputType);
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Page with that name already exists")]
        public void AddPage_AddFirstPageThatAlreadyExists_Throws_Exception_Xml()
        {
            OutputType outputType = OutputType.XML;
            AddPage_AddFirstPageThatlreadyExists(outputType);
        }

        private void AddPage_AddFirstPageThatlreadyExists(OutputType output)
        {
            string firstPageTitle = layout.GetPages(output).First().title;
            layout.AddPage(firstPageTitle, output: output);
        }

        #endregion

        #region DeletePage

        [Test]
        [ExpectedException(ExpectedMessage = "Invalid page id")]
        public void DeletePage_InvalidPageId_ThrowsException_Json()
        {
            OutputType outputType = OutputType.JSON;
            DeletePage(outputType);
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Invalid page id")]
        public void DeletePage_InvalidPageId_ThrowsException_Xml()
        {
            OutputType outputType = OutputType.XML;
            DeletePage(outputType);
        }

        private void DeletePage(OutputType output)
        {
            layout.DeletePage(-1);
        }

        #endregion

        #region DeletePageModule

        [Test]
        public void DeletePageModule_Json()
        {
            OutputType outputType = OutputType.JSON;
            DeletePageModule(outputType);
        }

        [Test]
        public void DeletePageModule_Xml()
        {
            OutputType outputType = OutputType.XML;
            DeletePageModule(outputType);
        }

        private void DeletePageModule(OutputType output)
        {
            AddPageAddNewExternalMonitorAddPageModuleDeleteMonitorDeletePage(output);
        }


        #endregion

        #region AddPageModule

        [Test]
        public void AddPageModule_AddPageAddNewExternalMonitorAddPageModuleDeleteModuleDeleteMonitorDeletePage_Json()
        {
            OutputType outputType = OutputType.JSON;
            AddPageAddNewExternalMonitorAddPageModuleDeleteMonitorDeletePage(outputType);
        }

        [Test]
        public void AddPageModule_AddPageAddNewExternalMonitorAddPageModuleDeleteModuleDeleteMonitorDeletePage_Xml()
        {
            OutputType outputType = OutputType.XML;
            AddPageAddNewExternalMonitorAddPageModuleDeleteMonitorDeletePage(outputType);
        }

        private void AddPageAddNewExternalMonitorAddPageModuleDeleteMonitorDeletePage(OutputType output)
        {
            //add page
            string pageName = "Test api page" + Common.GenerateRandomString(5);
            int pageId = layout.AddPage(pageName, 2, output);
            //add external monitor
            ExternalMonitor externalMonitor = new ExternalMonitor(authentication);
            int idMonitorToDelete = ExternalMonitorTests.AddMonitor(externalMonitor, output);

            //add module
            int pageModuleId = layout.AddPageModule(Layout.ModulName.External, pageId, 2, 2, idMonitorToDelete,
                                                    output: output);
            //delete page module
            layout.DeletePageModule(pageModuleId, output: output);
            //delete module
            externalMonitor.DeleteMonitors(testId: idMonitorToDelete, output: output);
            //delete page
            layout.DeletePage(pageId, output);
        }

        #endregion
    }
}
