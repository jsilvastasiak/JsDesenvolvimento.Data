using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using JsDesenvolvimento.Data.Repository;

namespace JsDesenvolvimento.Data.Common.Impl
{
    public class DefaultDbContext : IStandardDbContext
    {
        public DefaultDbContext() { }

        public DefaultDbContext(IDbConnection connection, IDbTransaction transaction, IServiceProvider provider)
        {
            InnerConnection = connection;
            InnerTransaction = transaction;
            Container = provider;
        }

        public IDbConnection InnerConnection { get; private set; }

        public IDbTransaction InnerTransaction { get; private set; }

        private IServiceProvider Container { get; set; }

        public T AcquireRepository<T>() where T : IRepository
        {
            T repo = (T)this.Container.GetService(typeof(T));
            repo.AttachContext(this);
            return repo;
        }

        public object AcquireRepository(Type type)
        {
            return this.Container.GetService(type);
        }

        public T AcquireService<T>()
        {
            return (T)this.Container.GetService(typeof(T));
        }

        public void AttachContext(IDbConnection connection, IDbTransaction transaction, IServiceProvider provider)
        {
            this.InnerConnection = connection;
            this.InnerTransaction = transaction;
            this.Container = provider;
        }

        public void Commit()
        {
            this.InnerTransaction.Commit();
        }

        public void Dispose()
        {
            if(this.InnerConnection.State != ConnectionState.Closed)
            {
                this.InnerConnection.Close();
            }
            
            this.InnerConnection.Dispose();
        }
    }
}
