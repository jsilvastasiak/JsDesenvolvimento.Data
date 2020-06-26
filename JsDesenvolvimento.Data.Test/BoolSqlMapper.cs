using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JsDesenvolvimento.Data.Test
{
    public class BoolSqlMapper : SqlMapper.TypeHandler<bool>
    {
        public override bool Parse(object value)
        {
            if (value is string)
                return value.ToString().Equals("S");

            if (value is int || value is long)
                return Convert.ToInt64(value) == 1;

            throw new ArgumentException("Valor retornado do banco impossível de transformar para tipo booleano.");
        }

        public override void SetValue(IDbDataParameter parameter, bool value)
        {
            parameter.DbType = DbType.String;
            parameter.Size = 1;
            parameter.Value = value ? "S" : "N";
        }
    }
}
