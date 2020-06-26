using System;
using System.Collections.Generic;
using System.Text;

namespace JsDesenvolvimento.Data
{
    public interface IDbContextFactory
    {
        IDbContext NewContext();

        IDbContext NewTransactionContext();
    }
}
