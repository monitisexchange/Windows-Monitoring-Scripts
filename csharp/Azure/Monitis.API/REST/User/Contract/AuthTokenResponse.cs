using System;
using Monitis.API.Common;

namespace Monitis.API.REST.User.Contract
{
    public class AuthTokenResponse : APIResponce
    {
        public String AuthToken { get; set; }
        
    }
}