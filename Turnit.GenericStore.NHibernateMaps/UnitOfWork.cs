using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.SqlCommand;
using System;
using System.Diagnostics;

namespace Turnit.GenericStore.NHibernateMaps
{
    public class UnitOfWork
    {
        public static string ConnectionString;
        public static ISessionFactory SessionFactory;
        public static ISessionFactory CreateSessionFactory(IServiceProvider context)
        {
            if (SessionFactory == null)
            {

                var configuration = Fluently.Configure()
                    .Database(PostgreSQLConfiguration.PostgreSQL82
                        .Dialect<NHibernate.Dialect.PostgreSQL82Dialect>()
                        .ConnectionString(ConnectionString))
                    .Mappings(x =>
                    {
                        x.FluentMappings.AddFromAssemblyOf<ProductMap>();
                    })
                    .ExposeConfiguration(c => c.SetInterceptor(new DebugInterceptor()));           


                SessionFactory = configuration.BuildSessionFactory();
            }

            return SessionFactory;
        }

    }

    public class DebugInterceptor : EmptyInterceptor
    {
        public override SqlString OnPrepareStatement(SqlString sql)
        {
            Debug.Write(sql.ToString());
            return sql;
        }
    }
}
