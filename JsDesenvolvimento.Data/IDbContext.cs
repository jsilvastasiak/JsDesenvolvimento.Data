using JsDesenvolvimento.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JsDesenvolvimento.Data
{
    public interface IDbContext : IDisposable
    {
        IDbConnection InnerConnection { get; }

        IDbTransaction InnerTransaction { get; }

        T AcquireRepository<T>() where T : IRepository;

        object AcquireRepository(Type type);

        T AcquireService<T>();

        void Commit();
    }
}
