using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace JsDesenvolvimento.Data.Common.Impl
{
    public class DefaultConnectionFactory : IDbConnectionFactory
    {
        private IDbProvider DbProvider;

        private string ConnectionString;

        public DefaultConnectionFactory(IDbProvider dbProvider, string connectionString)
        {
            DbProvider = dbProvider;
            ConnectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            DbProviderFactory factory = DbProvider.GetDbProvider();
            var conn = factory.CreateConnection();
            conn.ConnectionString = this.ConnectionString;

            return conn;
        }
    }
}
