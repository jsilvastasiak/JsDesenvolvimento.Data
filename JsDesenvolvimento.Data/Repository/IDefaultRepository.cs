using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;

namespace JsDesenvolvimento.Data.Repository
{
    public interface IDefaultRepository<TEntity> : IRepository where TEntity : class
    {
        Task<TEntity> InsertAsync(TEntity entidade, CancellationToken cancellationToken);

        Task<TEntity> UpdateAsync(TEntity entidade, CancellationToken cancellationToken);

        Task<IList<TEntity>> Fetch(IPredicate predicate, CancellationToken cancellationToken);

        Task<TEntity> FetchByKey(TEntity entity_key, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(TEntity entidade, CancellationToken cancellationToken);
    }
}
