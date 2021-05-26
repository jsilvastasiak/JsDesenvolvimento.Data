using DapperExtensions.Mapper;
using JsDesenvolvimento.Data.Test.Repositorios.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsDesenvolvimento.Data.Test
{
    public static class ModuloClassMapper
    {
        public class PessoaMapper : ClassMapper<Pessoa>
        {
            public PessoaMapper()
            {
                Table("pessoa");

                Map(a => a.idpessoa).Column("idpessoa");
                Map(a => a.tipoinscricao).Column("tipoinscricao");
                Map(a => a.cpf).Column("cpf");
                Map(a => a.cnpj).Column("cnpj");
                Map(a => a.NomeQualquer).Column("nome");
                Map(a => a.email).Column("email");
            }
        }

        public class PessoaEnderecoMapper : ClassMapper<PessoaEndereco>
        {
            public PessoaEnderecoMapper()
            {
                Table("pessoa_endereco");

                Map(a => a.idpessoa).Column("idpessoa");
                Map(a => a.id).Column("id");
                Map(a => a.rua).Column("rua");
                Map(a => a.bairro).Column("bairro");
                Map(a => a.cidade).Column("cidade");
                Map(a => a.uf).Column("uf");
                Map(a => a.cep).Column("cep");
                Map(a => a.numero).Column("numero");
                Map(a => a.complemento).Column("complemento");
                Map(a => a.telefone).Column("telefone");
                Map(a => a.ddd).Column("ddd");
            }
        }
    }
}
