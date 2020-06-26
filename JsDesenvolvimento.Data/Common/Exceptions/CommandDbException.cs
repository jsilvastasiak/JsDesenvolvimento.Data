using System;
using System.Collections.Generic;
using System.Text;

namespace JsDesenvolvimento.Data.Common.Exceptions
{
    public class CommandDbException : Exception
    {
        public CommandDbException(string mensagem): base(mensagem)
        {

        }
    }
}
