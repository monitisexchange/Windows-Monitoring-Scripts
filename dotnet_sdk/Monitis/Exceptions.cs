using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monitis
{
    public class Exceptions
    {
        public class ApiKeyIsNullOrEmptyException : Exception
        {
            public override string Message { get { return "ApiKey is null or empty!"; } }
        }

        public class SecretKeyIsNullOrEmptyException : Exception
        {
            public override string Message { get { return "SecretKey is null or empty!"; } }
        }

        public class AuthTokenisNullOrEmptyException : Exception
        {
            public override string Message { get { return "AuthToken is null or empty!"; } }
        }

        public class ParamsNotSpecifiedOrHasNullValuesException : Exception
        {
            public override string Message { get { return "Params not specified or has null values!"; } }
        }

        public class MonitisException : Exception
        {
            public MonitisException(string s, Exception exception) : base(s, exception)
            {
                
            }
        }
    }
}
