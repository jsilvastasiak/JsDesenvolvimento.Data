using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsDesenvolvimento.Data.Test.Repositorios.Model
{
    [Table("pessoa_endereco")]
    public class PessoaEndereco
    {
        [ExplicitKey]
        public int idpessoa { get; set; }
        [Key]
        public int id { get; set; }
        public string rua { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string telefone { get; set; }
        public string ddd { get; set; }
    }
}
