using JsDesenvolvimento.Data.Common.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsDesenvolvimento.Data.Postgresql.Impl
{
    public class DefaultPostgresqlDbProvider : DefaultDbProvider, IDbProvider
    {
        public DefaultPostgresqlDbProvider(string invarianteNameProvider) : base(invarianteNameProvider)
        {
            Factory.RegisterPostgresqlFactory.Register(invarianteNameProvider);
        }
    }
}
