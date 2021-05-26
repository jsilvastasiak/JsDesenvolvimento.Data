using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using Dapper.Contrib.Extensions;
using JsDesenvolvimento.Data.Common.Exceptions;
using DapperExtensions.Sql;
using JsDesenvolvimento.Data.Common.Model;
using DapperExtensions.Mapper;

namespace JsDesenvolvimento.Data.Repository.Impl
{
    public abstract class AbstractDefaultRepository<T> where T : class
    {
        public const string PREDICADO = "tab";

        public IDbContext AttachedContext { get; private set; }

        public AbstractDefaultRepository(){ }
        
        public void AttachContext(IDbContext context)
        {
            AttachedContext = context;
        }

        public virtual async Task<T> InsertAsync(T entidade, CancellationToken cancellationToken)
        {
            EntidadeKey idIncrementado = Common.SqlBuilder.MapperTableKeys<T>.GetAutoIncrementedKey(entidade, this.AttachedContext);
            if(idIncrementado != null)
                entidade.GetType().GetProperty(idIncrementado.NomeColuna).SetValue(entidade, idIncrementado.KeyValue);

            IDictionary<string, object> parametros = new Dictionary<string, object>();
            string insertSql = Common.SqlBuilder.MapperEntitySql<T>.GetInsertSql(entidade, parametros);
            CommandDefinition cmd = new CommandDefinition(insertSql, parameters: parametros, transaction: this.AttachedContext.InnerTransaction, cancellationToken: cancellationToken);
            IList<EntidadeKey> keys = Common.SqlBuilder.MapperTableKeys<T>.GetWhereKeys(entidade);

            var result = await AttachedContext.InnerConnection.ExecuteScalarAsync(cmd);
            entidade.GetType().GetProperty(keys.Where(a => a.IsKey).First().NomeColuna).SetValue(entidade, result);
            return entidade;
        }

        /// <summary>
        /// Atualiza registro pelo objeto do parametro passado
        /// </summary>
        /// <param name="entidade">Objeto para ser atualizado</param>
        /// <param name="cancellationToken">Token de Cancelamento</param>
        /// <returns>Entidade Atualizada</returns>
        public virtual async Task<T> UpdateAsync(T entidade, CancellationToken cancellationToken)
        {
            IDictionary<string, object> parametros = new Dictionary<string, object>();
            string updateSql = Common.SqlBuilder.MapperEntitySql<T>.GetUpdateSql(entidade, parametros);
            CommandDefinition cmd = new CommandDefinition(updateSql, parameters: parametros, transaction: this.AttachedContext.InnerTransaction, cancellationToken: cancellationToken);

            var result = await AttachedContext.InnerConnection.ExecuteAsync(cmd);
            return entidade;
        }

        public virtual async Task<IList<T>> Fetch(IPredicate predicate, CancellationToken cancellationToken)
        {
            IDictionary<string, object> parametros = new Dictionary<string, object>();
            ISqlGenerator sqlGenerator = this.AttachedContext.AcquireService<ISqlGenerator>();
            IClassMapper classMapper = sqlGenerator.Configuration.GetMap<T>();
                        
            string selectGerado = sqlGenerator.Select(classMapper, predicate, null, parametros);
            string sqlMain = selectGerado;

            var result = await AttachedContext.InnerConnection.QueryAsync<T>(sqlMain, param: parametros, transaction: this.AttachedContext.InnerTransaction);
            return result.ToList();
        }

        public virtual async Task<T> FetchByKey(T entity_keys, CancellationToken cancellationToken)
        {
            IList<EntidadeKey> entidadeKeys = Common.SqlBuilder.MapperTableKeys<T>.GetWhereKeys(entity_keys);
            ISqlGenerator sqlGenerator = this.AttachedContext.AcquireService<ISqlGenerator>();
            IPredicate whereClause = Predicates.Group(GroupOperator.And, entidadeKeys.Select(a => Predicates.Field<T>(ent => ent.GetType().GetProperty(a.NomeColuna), Operator.Eq, a.KeyValue)).ToArray());
            IClassMapper classMapper = sqlGenerator.Configuration.GetMap<T>();
            IDictionary<string, object> parametros = new Dictionary<string, object>();
            string sqlGerado = sqlGenerator.Select(classMapper, whereClause, null, parametros);
            string sqlMain = sqlGerado;

            var result = await AttachedContext.InnerConnection.QueryFirstAsync<T>(sqlMain, param: parametros, transaction: this.AttachedContext.InnerTransaction);
            return result;
        }

        /// <summary>
        /// Deleta Registro pela Chave
        /// </summary>
        /// <param name="entidade"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(T entidade, CancellationToken cancellationToken)
        {
            IDictionary<string, object> parametros = new Dictionary<string, object>();
            string deleteSql = Common.SqlBuilder.MapperEntitySql<T>.GetDeleteSql(entidade, parametros);
            CommandDefinition cmd = new CommandDefinition(deleteSql, parameters: parametros, transaction: this.AttachedContext.InnerTransaction, cancellationToken: cancellationToken);

            var result = await AttachedContext.InnerConnection.ExecuteAsync(cmd);
            return result > 0;
        }
    }
}
