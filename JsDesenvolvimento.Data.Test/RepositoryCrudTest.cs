using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Autofac;
using JsDesenvolvimento.Data.Test.Repositorios;
using System.Threading;
using JsDesenvolvimento.Data.Common.SqlBuilder;
using JsDesenvolvimento.Data.Test.Repositorios.Model;
using System.Data;
using FakeItEasy;
using JsDesenvolvimento.Data.Test.Repositorios.Impl;
using System.Data.Common;
using JsDesenvolvimento.Data.Common.Impl;
using Dapper;
using DapperExtensions;
using JsDesenvolvimento.Data.Common.Model;

namespace JsDesenvolvimento.Data.Test
{
    public class RepositoryCrudTest: TestBase
    {
        [Fact]
        public void MapperTableKeysTest()
        {
            using (var ctx = this.AutofacContainer.Resolve<IDbContextFactory>().NewContext())
            {
                var entidadeKey = MapperTableKeys<PessoaEndereco>.GetAutoIncrementedKey(new PessoaEndereco()
                {
                    idpessoa = 1,
                    rua = "JacobJunkes",
                    uf = "SC"
                }, ctx);

                Assert.Equal("id", entidadeKey.NomeColuna);
                Assert.Equal(2, (int)entidadeKey.KeyValue);
            }
        }

        [Fact]
        public void FetchTest()
        {
            using (var ctx = this.AutofacContainer.Resolve<IDbContextFactory>().NewContext())
            {
                var repo = ctx.AcquireRepository<IPessoaEnderecoRepository>();
                var result = repo.Fetch(Predicates.Field<PessoaEndereco>(a => a.idpessoa, Operator.Eq, 1), CancellationToken.None).Result;

                Assert.True(result.Count > 0);
            }
        }

        [Fact]
        public void FetchPropertyMapperTest()
        {
            using (var ctx = this.AutofacContainer.Resolve<IDbContextFactory>().NewContext())
            {
                var repo = ctx.AcquireRepository<IPessoaRepository>();
                var result = repo.Fetch(Predicates.Field<Pessoa>(a => a.idpessoa, Operator.Eq, 1), CancellationToken.None).Result;

                Assert.True(result.Count > 0);
                Assert.True(result[0].NomeQualquer != null);
            }
        }

        [Fact]
        public void SetIdValueTest()
        {
            using (var ctx = this.AutofacContainer.Resolve<IDbContextFactory>().NewContext())
            {
                var entidade = new PessoaEndereco()
                {
                    idpessoa = 1
                };

                EntidadeKey idIncrementado = Common.SqlBuilder.MapperTableKeys<PessoaEndereco>.GetAutoIncrementedKey(entidade, ctx);
                if (idIncrementado != null)
                    entidade.GetType().GetProperty(idIncrementado.NomeColuna).SetValue(entidade, idIncrementado.KeyValue);

                Assert.Equal(2, entidade.id);
            } 
        }

        [Fact]
        public void InsertAsyncTest()
        {
            using (var ctx = this.AutofacContainer.Resolve<IDbContextFactory>().NewTransactionContext())
            {
                var entidade = new PessoaEndereco()
                {
                    idpessoa = 1,
                    rua = "Florença",
                    bairro = "Santa Terezinha",
                    cidade = "Gaspar",
                    uf = "SC",
                    cep = "89110340",
                    ddd = "47",
                    numero = "22",
                    telefone = "985759656",
                    complemento = "Casa"
                };

                var repo = ctx.AcquireRepository<IPessoaEnderecoRepository>();
                var ent = repo.InsertAsync(entidade, CancellationToken.None).Result;

                Assert.Equal(2, ent.id);
            }
        }

        [Fact]
        public void InsertPessoaAsyncTest()
        {
            using (var ctx = this.AutofacContainer.Resolve<IDbContextFactory>().NewTransactionContext())
            {
                var entidade = new Pessoa()
                {
                   cpf = "45478945555",
                   email = "lucas.stasiaky@hotmail.com",
                   NomeQualquer = "Lucas",
                   senha = "1234",
                   tipoinscricao = "F"
                };

                var repo = ctx.AcquireRepository<IPessoaRepository>();
                var ent = repo.InsertAsync(entidade, CancellationToken.None).Result;

                Assert.Equal(4, ent.idpessoa);
            }
        }

        protected override void ConfigFakeObjects(ContainerBuilder builder)
        {
            builder.Register(a => new DefaultPessoEnderecoRepository()).As<IPessoaEnderecoRepository>();
            builder.Register(a => new DefaultPessoaRepository()).As<IPessoaRepository>();
            //builder.Register<IDbProvider>(a => GetDbProviderFake()).AsImplementedInterfaces().SingleInstance();
        }

        protected IDbProvider GetDbProviderFake()
        {
            var dbProvider = new Fake<IDbProvider>(a => a.CallsBaseMethods());
            dbProvider.CallsTo(a => a.GetDbProvider()).ReturnsLazily(a => GetDbProviderFactoryFake());
            Factory.RegisterPostgresqlFactory.Register("Npgsql");
            return dbProvider.FakedObject;
        }

        protected DbProviderFactory GetDbProviderFactoryFake()
        {
            var idbDataReaderFake = new Fake<DbDataReader>(a => a.CallsBaseMethods());

            var idCommand = new Fake<DbCommand>(a => a.CallsBaseMethods());
            idCommand.CallsTo(a => a.ExecuteScalar()).Returns(new { id = 1, idpessoa = 1, cpf = "5456647896" });
            idCommand.CallsTo(a => a.ExecuteNonQuery()).Returns(2);
            //idCommand.CallsTo(a => a.ExecuteDbDataReader(A<CommandBehavior>.Ignored)).ReturnsLazily(() => GetReaderFake());

            var dbProviderFake = new Fake<DbProviderFactory>(a => DbProviderFactories.GetFactory("Npgsql"));
            dbProviderFake.CallsTo(a => a.CreateCommand()).Returns(idCommand.FakedObject);
            
            return dbProviderFake.FakedObject;
        }

        protected DbDataReader GetReaderFake()
        {
            return new Fake<DbDataReader>(a => a.CallsBaseMethods()).FakedObject;
        }
    }
}
