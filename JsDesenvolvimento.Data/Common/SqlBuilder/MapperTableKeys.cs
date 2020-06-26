using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;
using Dapper;
using DapperExtensions;
using JsDesenvolvimento.Data.Common.Exceptions;
using JsDesenvolvimento.Data.Postgresql;
using JsDesenvolvimento.Data.Common.Model;

namespace JsDesenvolvimento.Data.Common.SqlBuilder
{
    public static class MapperTableKeys<T> where T : class
    {
        /// <summary>
        /// Faz consulta na base de dados caso chave seja complexa e traz campo key com valor autoincrementado
        /// </summary>
        /// <param name="entidade">Entidade</param>
        /// <param name="AttachedContext">Contexto e banco</param>
        /// <returns>Chave AutoIncrementado</returns>
        public static EntidadeKey GetAutoIncrementedKey(T entidade, IDbContext AttachedContext)
        {
            var tipoEntidade = typeof(T);
            var atributo = (TableAttribute)Attribute.GetCustomAttribute(tipoEntidade, typeof(TableAttribute));

            var pkEstrangeira = tipoEntidade.GetProperties().Where(a => a.CustomAttributes.Where(atrr => atrr.AttributeType == typeof(ExplicitKeyAttribute)).Count() > 0).Select(a => a);
            var pkAutoIncrement = tipoEntidade.GetProperties().Where(a => a.CustomAttributes.Where(atrr => atrr.AttributeType == typeof(KeyAttribute)).Count() > 0).Select(a => a);

            if (atributo == null || string.IsNullOrEmpty(atributo.Name))
                throw new CommandDbException("Informações de tabela não encontradas na Entidade");

            //Se possuir as duas chaves, quer dizer que a Key tem que ser por autoincrement
            if(pkEstrangeira.Count() > 0 && pkAutoIncrement.Count() > 0)
            {
                string campoAutoInc = pkAutoIncrement.First().Name;
                IList<string> camposFK = pkEstrangeira.Select(a => $@"aux.{a.Name} = { entidade.GetType().GetProperty(a.Name).GetValue(entidade, null) }").ToList();
                string sqlPrincipal = $@"(SELECT MAX(aux.{ campoAutoInc})
                                            FROM { atributo.Name} aux
                                           WHERE { string.Join(" AND ", camposFK) })";
                string sql = SqlPostgresBuilderHelper.GetSelectAutoIncrement("valor", sqlPrincipal);

                object key = AttachedContext.InnerConnection.QueryFirst(sql, transaction: AttachedContext.InnerTransaction).valor;
                return new EntidadeKey()
                {
                     KeyValue = (int)key,
                     NomeColuna = campoAutoInc
                };
            }

            return null;
        }

        /// <summary>
        /// Recupera todos campos mapeados que são chave da tabela
        /// </summary>
        /// <param name="entidade">Objeto com os valores</param>
        /// <returns>Lista de Chaves</returns>
        public static IList<EntidadeKey> GetWhereKeys(T entidade)
        {
            var tipoEntidade = typeof(T);
            var pks = tipoEntidade.GetProperties().Where(a => a.CustomAttributes.Where(atrr => atrr.AttributeType == typeof(ExplicitKeyAttribute) || atrr.AttributeType == typeof(KeyAttribute)).Count() > 0).Select(a => new EntidadeKey()
            {
                KeyValue = entidade.GetType().GetProperty(a.Name).GetValue(entidade, null),
                NomeColuna = a.Name,
                IsKey = a.CustomAttributes.First().AttributeType == typeof(KeyAttribute)
            });

            return pks.ToList();
        }

        /// <summary>
        /// Seleciona o nome da tabela do banco de dados
        /// </summary>
        /// <param name="entidade">Objeto com os valores</param>
        /// <returns>nome da tabela</returns>
        public static string GetTableName()
        {
            var tipoEntidade = typeof(T);
            var atributo = (TableAttribute)Attribute.GetCustomAttribute(tipoEntidade, typeof(TableAttribute));

            if (atributo == null || string.IsNullOrEmpty(atributo.Name))
                throw new CommandDbException("Informações de tabela não encontradas na Entidade");

            return atributo.Name;
        }
    }
}
