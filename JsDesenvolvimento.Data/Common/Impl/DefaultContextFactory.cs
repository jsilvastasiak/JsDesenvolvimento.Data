using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JsDesenvolvimento.Data.Common.Impl
{
    public class DefaultContextFactory<T> : IDbContextFactory where T : IStandardDbContext, new()
    {
        private IDbConnectionFactory Factory { get; set; }

        private IServiceProvider ServiceLocator { get; set; }

        public DefaultContextFactory(IServiceProvider container, IDbConnectionFactory factory)
        {
            this.Factory = factory;
            this.ServiceLocator = container;
        }

        public IDbContext NewContext()
        {
            var conn = Factory.CreateConnection();
            conn.Open();
            T ctx = new T();
            ctx.AttachContext(conn, null, ServiceLocator);
            return ctx;
        }

        public IDbContext NewTransactionContext()   
        {
            var conn = Factory.CreateConnection();
            conn.Open();
            var transaction = conn.BeginTransaction();
            T ctx = new T();
            ctx.AttachContext(conn, transaction, ServiceLocator);
            return ctx;
        }
    }
}
