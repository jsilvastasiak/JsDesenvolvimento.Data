using Autofac;
using Dapper;
using DapperExtensions.Mapper;
using JsDesenvolvimento.Data.Common.Impl;
using JsDesenvolvimento.Data.Postgresql.Impl;
using JsDesenvolvimento.Data.Test.Repositorios;
using JsDesenvolvimento.Data.Test.Repositorios.Impl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FakeItEasy;
using Autofac.Extensions.DependencyInjection;

namespace JsDesenvolvimento.Data.Test
{
    public abstract class TestBase
    {
        protected IContainer AutofacContainer { get; set; }

        public TestBase()
        {
            // Create a container-builder and register dependencies
            var builder = new ContainerBuilder();

            // Populate the service-descriptors added to `IServiceCollection`
            // BEFORE you add things to Autofac so that the Autofac
            // registrations can override stuff in the `IServiceCollection`
            // as needed
            //builder.Populate(services);

            ConfigureContainer(builder);
            AutofacContainer = builder.Build();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            ConfigurarProviderBanco(builder);
            ConfigFakeObjects(builder);
        }

        private static DapperExtensions.Sql.ISqlDialect GetDialeto()
        {
            return new DapperExtensions.Sql.PostgreSqlDialect();
        }

        public void ConfigurarProviderBanco(ContainerBuilder builder)
        {
            DapperExtensions.DapperExtensions.ClearCache();
            DapperExtensions.Sql.ISqlDialect sqlDialect = GetDialeto();

            DapperExtensions.IDapperExtensionsConfiguration config = new DapperExtensions.DapperExtensionsConfiguration(
                typeof(AutoClassMapper<>),
                new[]
                {
                    typeof(Data.Repository.Model.Loja).Assembly,                    
                },
                GetDialeto()
            );
            DapperExtensions.DapperExtensions.Configure(config);

            SqlMapper.RemoveTypeMap(typeof(bool));
            SqlMapper.AddTypeHandler(typeof(bool), new BoolSqlMapper());

            builder.Register<DapperExtensions.Sql.ISqlGenerator>(a => new DapperExtensions.Sql.SqlGeneratorImpl(config));

            builder.Register<IDbProvider>(ctx => new DefaultPostgresqlDbProvider("Npgsql")).AsImplementedInterfaces().SingleInstance();
            //Conexão com banco
            builder.Register<IDbConnectionFactory>(ctx => new DefaultConnectionFactory(ctx.Resolve<IDbProvider>(), "host=localhost;port=1234;database=Eshopping;user id=postgres;password=1234;")).AsImplementedInterfaces();

            builder.Register<IDbContextFactory>(ctx => new DefaultContextFactory<DefaultDbContext>(new AutofacServiceProvider(AutofacContainer), ctx.Resolve<IDbConnectionFactory>())).AsImplementedInterfaces();
        }

        protected abstract void ConfigFakeObjects(ContainerBuilder builder);
    }
}
