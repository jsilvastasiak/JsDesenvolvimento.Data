using System;
using System.Collections.Generic;
using System.Text;

namespace JsDesenvolvimento.Data.Postgresql
{
    public static class SqlPostgresBuilderHelper
    {
        public static string GetSelectAutoIncrement(string nomeColuna, string sqlMain = null)
        {
            return $@"SELECT COALESCE({sqlMain} + 1, 1) {nomeColuna}";
        }
    }
}
