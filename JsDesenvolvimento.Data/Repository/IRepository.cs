using System;
using System.Collections.Generic;
using System.Text;

namespace JsDesenvolvimento.Data.Repository
{
    public interface IRepository
    {
        IDbContext AttachedContext { get; }

        void AttachContext(IDbContext context);
    }
}
