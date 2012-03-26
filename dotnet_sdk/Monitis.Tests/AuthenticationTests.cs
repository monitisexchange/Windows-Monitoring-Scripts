using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Monitis.Tests
{
    [TestFixture]
    public class AuthenticationTests
    {
        #region Authenticate

        [Test]
        public void Authenticate_Valid_XML_ReturnsTrue()
        {
            var authentication = new Authentication();
            authentication.Authenticate(MonitisAccountInformation.Login, MonitisAccountInformation.Password, OutputType.XML);
            Assert.IsTrue((
                MonitisAccountInformation.ApiKey == authentication.apiKey&&
                MonitisAccountInformation.SekretKey == authentication.secretKey&&
                !string.IsNullOrEmpty(authentication.authToken)), "Authenticate (XML output) returns invalid value.");
        }

        [Test]
        public void Authenticate_Valid_Json_ReturnsTrue()
        {
            var authentication = new Authentication();
            authentication.Authenticate(MonitisAccountInformation.Login, MonitisAccountInformation.Password, OutputType.JSON);
            Assert.IsTrue((
                MonitisAccountInformation.ApiKey == authentication.apiKey &&
                MonitisAccountInformation.SekretKey == authentication.secretKey &&
                !string.IsNullOrEmpty(authentication.authToken)), "Authenticate (XML output) returns invalid value.");
        }

        #endregion

        #region GetApiKey

        [Test]
        public void GetApiKey_ValidApiKey_Xml()
        {
            var authentication = new Authentication();
            var apiKey= authentication.GetApiKey(MonitisAccountInformation.Login, MonitisAccountInformation.Password, OutputType.XML);
            Assert.IsTrue(apiKey==MonitisAccountInformation.ApiKey,"Apikey invalid");
        }

        [Test]
        public void GetApiKey_ValidApiKey_Json()
        {
            var authentication = new Authentication();
            var apiKey = authentication.GetApiKey(MonitisAccountInformation.Login, MonitisAccountInformation.Password, OutputType.JSON);
            Assert.IsTrue(apiKey == MonitisAccountInformation.ApiKey, "Apikey invalid");
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Invalid username or password")]
        public void GetApiKey_InvalidUserOrPassword_Xml_ThrowsException()
        {
            var authentication = new Authentication();
            authentication.GetApiKey(MonitisAccountInformation.Login + "ERROR!", MonitisAccountInformation.Password + "ERROR!", OutputType.XML);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Invalid username or password")]
        public void GetApiKey_InvalidUserOrPassword_Json_ThrowsException()
        {
            var authentication = new Authentication();
            authentication.GetApiKey(MonitisAccountInformation.Login + "ERROR!", MonitisAccountInformation.Password + "ERROR!", OutputType.XML);
        }



        #endregion

        #region GetSecretKey

        [Test]
        public void GetSecretKey_ValidSecretKeyWithBaseApiKey_Xml()
        {
            var authentication = new Authentication();
            authentication.apiKey = MonitisAccountInformation.ApiKey;
            var secretKey = authentication.GetSecretKey(output: OutputType.XML);
            Assert.IsTrue(secretKey == MonitisAccountInformation.SekretKey, "SecretKey invalid");
        }

        [Test]
        public void GetSecretKey_ValidSecretKeyWithBaseApiKey_Json()
        {
            var authentication = new Authentication();
            authentication.apiKey = MonitisAccountInformation.ApiKey;
            var secretKey = authentication.GetSecretKey(output: OutputType.JSON);
            Assert.IsTrue(secretKey == MonitisAccountInformation.SekretKey, "SecretKey invalid");
        }

        [Test]
        public void GetSecretKey_ValidSecretKeyWithSpecifiedApiKey_Xml()
        {
            var authentication = new Authentication();
            var secretKey = authentication.GetSecretKey(apikey:MonitisAccountInformation.ApiKey, output: OutputType.XML);
            Assert.IsTrue(secretKey == MonitisAccountInformation.SekretKey, "SecretKey invalid");
        }

        [Test]
        public void GetSecretKey_ValidSecretKeyWithSpecifiedApiKey_Json()
        {
            var authentication = new Authentication();
            var secretKey = authentication.GetSecretKey(apikey:MonitisAccountInformation.ApiKey,output: OutputType.JSON);
            Assert.IsTrue(secretKey == MonitisAccountInformation.SekretKey, "SecretKey invalid");
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Authentication failure")]
        public void GetSecretKey_AuthError_Xml_ThrowsException()
        {
            var authentication = new Authentication();
            authentication.GetSecretKey(output: OutputType.XML);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Authentication failure")]
        public void GetSecretKey_AuthError_Json_ThrowsException()
        {
            var authentication = new Authentication();
            authentication.GetSecretKey(output: OutputType.JSON);
        }

        #endregion

        #region GetAuthToken

        [Test]
        public void GetAuthToken_AuthTokenIsNotNull_Json()
        {
            var authentication = new Authentication();
            authentication.apiKey = MonitisAccountInformation.ApiKey;
            authentication.secretKey = MonitisAccountInformation.SekretKey;
            var authToken = authentication.GetAuthToken(output: OutputType.JSON);
            Assert.IsNotNullOrEmpty(authToken);
        }

        [Test]
        public void GetAuthToken_AuthTokenIsNotNull_Xml()
        {
            var authentication = new Authentication();
            authentication.apiKey = MonitisAccountInformation.ApiKey;
            authentication.secretKey = MonitisAccountInformation.SekretKey;
            var authToken = authentication.GetAuthToken(output: OutputType.XML);
            Assert.IsNotNullOrEmpty(authToken);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Authentication failure")]
        public void GetAuthToken_AuthException_Json_ThrowsException()
        {
            var authentication = new Authentication();
            authentication.GetAuthToken(output: OutputType.JSON);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Authentication failure")]
        public void GetAuthToken_AuthException_Xml_ThrowsException()
        {
            var authentication = new Authentication();
            authentication.GetAuthToken(output: OutputType.XML);
        }

        #endregion
    }
}
