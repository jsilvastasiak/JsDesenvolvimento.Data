using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsDesenvolvimento.Data.Test.Repositorios.Model
{
    [Table("pessoa")]
    public class Pessoa
    {
        [Key]
        public int idpessoa { get; set; }
        public string tipoinscricao { get; set; }
        public string cpf { get; set; }
        public string cnpj { get; set; }
        public string NomeQualquer { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
    }
}
