using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monitis.Tests
{
    /// <summary>
    /// Your monitis account information. Fill it before run tests.
    /// </summary>
    public static class MonitisAccountInformation
    {
        public static string Login = "monitis_test@adoriasoft.com";
        public static string Password = "SuperPassword!";
        public static string ApiKey = "3HQB2PHSMHCRRHV5M9FR8MTBC";
        public static string SekretKey = "1H4AMF6FEO72IEVH8FD5B4US61";
        public static string FirstName = "monitis";
        public static string LastName = "test";

        /// <summary>
        /// You can see this pages on the top of your account in tabs
        /// </summary>
        public static string[] PagesInYourAccount = new string[] { "Summary", "Tools", "Monitors", "Notifications", "Web Map" };
    }
}
