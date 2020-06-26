using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using JsDesenvolvimento.Data.Common.Model;

namespace JsDesenvolvimento.Data.Common.SqlBuilder
{
    public static class MapperEntitySql<T> where T : class
    {
        public static string GetInsertSql(T entidade, IDictionary<string, object> parametros)
        {
            StringBuilder sql = new StringBuilder();
            IList<EntidadeKey> keys = MapperTableKeys<T>.GetWhereKeys(entidade);
            PropertyInfo[] properties = null;

            //Se entidade possui uma unica chave, entendesse que a tabela está com sequence
            if (keys.Count == 1 && keys.First().IsKey)
                properties = typeof(T).GetProperties().Where(a => !a.Name.Equals(keys.First().NomeColuna)).Select(a => a).ToArray();
            else
                properties = typeof(T).GetProperties();

            sql.Append($@"INSERT INTO {MapperTableKeys<T>.GetTableName()}");
            sql.Append("(");
            sql.Append(string.Join(", ", properties.Select(a => a.Name)));
            sql.Append(")");
            sql.Append(" VALUES (");
            sql.Append(string.Join(", ", properties.Select(a => ":prm" + a.Name)));
            sql.Append(") RETURNING ");
            sql.Append(keys.Where(a => a.IsKey).First().NomeColuna);

            if (parametros == null)
                throw new ArgumentNullException("Dicionário de Parametros não inicializado");

            foreach (var item in properties)
            {
                parametros.Add("prm" + item.Name, item.GetValue(entidade, null));
            }

            return sql.ToString();
        }

        public static string GetUpdateSql(T entidade, IDictionary<string, object> parametros)
        {
            StringBuilder sql = new StringBuilder();
            IList<EntidadeKey> keys = MapperTableKeys<T>.GetWhereKeys(entidade).ToList();

            sql.Append($@"UPDATE {MapperTableKeys<T>.GetTableName()} ");
            sql.Append("SET ");
            PropertyInfo[] properties = typeof(T).GetProperties();
            sql.Append(string.Join(", ", properties.Select(a => a.Name + "=:prm" + a.Name)));
            sql.Append(" WHERE ");
            sql.Append(string.Join(" AND ", keys.Select(a => a.NomeColuna + "=:prm" + a.NomeColuna)));

            if (parametros == null)
                throw new ArgumentNullException("Dicionário de Parametros não inicializado");

            foreach (var item in properties)
            {
                parametros.Add("prm" + item.Name, item.GetValue(entidade, null));
            }

            return sql.ToString();
        }

        public static string GetDeleteSql(T entidade, IDictionary<string, object> parametros)
        {
            StringBuilder sql = new StringBuilder();
            IList<EntidadeKey> keys = MapperTableKeys<T>.GetWhereKeys(entidade).ToList();

            sql.Append($@"DELETE FROM {MapperTableKeys<T>.GetTableName()} ");
            sql.Append(" WHERE ");
            sql.Append(string.Join(" AND ", keys.Select(a => a.NomeColuna + "=:prm" + a.NomeColuna)));

            if (parametros == null)
                throw new ArgumentNullException("Dicionário de Parametros não inicializado");

            foreach (var item in keys)
            {
                parametros.Add("prm" + item.NomeColuna, item.KeyValue);
            }

            return sql.ToString();
        }
    }
}
