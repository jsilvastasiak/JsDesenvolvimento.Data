using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace JsDesenvolvimento.Data.Factory
{
    public static class RegisterPostgresqlFactory
    {
        public static void Register(string providerName)
        {
            DbProviderFactories.RegisterFactory(providerName, Npgsql.NpgsqlFactory.Instance);
        }
    }
}
