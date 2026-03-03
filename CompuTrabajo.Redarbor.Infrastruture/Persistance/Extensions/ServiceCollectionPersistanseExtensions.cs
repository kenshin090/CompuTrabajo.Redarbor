using CompuTrabajo.Redarbor.Infrastruture.Persistance.Extensions.Configuration;
using CompuTrabajo.Redarbor.Infrastruture.Persistance.Interceptors;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CompuTrabajo.Redarbor.Infrastruture.Persistance.Extensions
{
    public static class ServiceCollectionPersistanseExtensions
    {
        public static IServiceCollection AddSqlServerPersistence<TContext>(
        this IServiceCollection services,
        IConfiguration config,
        SqlServerPersistenceOptions options)
        where TContext : DbContext
        {
            services.AddSingleton<IClock, SystemClock>();
            services.AddScoped<AuditingSaveChangesInterceptor>();

            services.AddDbContext<TContext>((sp, db) =>
            {
                var cs = config.GetConnectionString(options.ConnectionStringName)
                         ?? throw new InvalidOperationException($"Missing connection string: {options.ConnectionStringName}");



                db.UseSqlServer(
     cs,
     b => b.MigrationsAssembly("CompuTrabajo.Redarbor.Infrastruture"));

                db.EnableDetailedErrors(options.EnableDetailedErrors);
                db.EnableSensitiveDataLogging(options.EnableSensitiveDataLogging);

                db.AddInterceptors(
                    sp.GetRequiredService<AuditingSaveChangesInterceptor>()
                );
            });

            services.AddScoped<IDbConnection>(sp =>
            {
                var cs = config.GetConnectionString(options.ConnectionStringName)
                         ?? throw new InvalidOperationException(
                             $"Missing connection string: {options.ConnectionStringName}");

                return new SqlConnection(cs);
            });

            return services;
        }
    }
}
