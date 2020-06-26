using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JsDesenvolvimento.Data
{
    public interface IStandardDbContext : IDbContext
    {
        void AttachContext(IDbConnection connection, IDbTransaction transaction, IServiceProvider provider);
    }
}
