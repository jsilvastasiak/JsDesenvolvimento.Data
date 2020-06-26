using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace JsDesenvolvimento.Data
{
    public interface IDbProvider
    {
        DbProviderFactory GetDbProvider();
    }
}
