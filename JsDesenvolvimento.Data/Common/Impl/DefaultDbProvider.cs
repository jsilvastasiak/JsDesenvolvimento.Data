using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JsDesenvolvimento.Data.Common.Impl
{
    public abstract class DefaultDbProvider
    {
        protected string InvariantNameProvider { get; set; }

        public DefaultDbProvider(string invarianteNameProvider)
        {
            if (string.IsNullOrEmpty(invarianteNameProvider))
                throw new ArgumentException("O nome do Provider não pode ser null");

            this.InvariantNameProvider = invarianteNameProvider;
        }

        public virtual DbProviderFactory GetDbProvider()
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(this.InvariantNameProvider);

            if(factory == null)
                throw new ArgumentException("Provider não encontrado no computador local");

            return factory;
        }
    }
}
