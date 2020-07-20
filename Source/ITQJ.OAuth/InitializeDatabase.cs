﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Linq;

namespace ITQJ.OAuth
{
    public static class InitializeDatabase
    {
        public static bool IsDataFetched(this ConfigurationDbContext context)
        {
            try
            {
                return context.Clients.Any();
            }
            catch (Exception ex)
            {
                Log.Error("Database data is not fetched.\nError: {0}.", ex.Message);
                return false;
            }
        }

        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddOperationalDbContext(options =>
            {
                options.ConfigureDbContext = db => db.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(InitializeDatabase).Assembly.FullName));
            });
            services.AddConfigurationDbContext(options =>
            {
                options.ConfigureDbContext = db => db.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(InitializeDatabase).Assembly.FullName));
            });

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    Log.Debug("Clients being populated");
                    foreach (var client in Config.Clients.ToList())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }
                else
                {
                    Log.Debug("Clients already populated");
                }

                if (!context.IdentityResources.Any())
                {
                    Log.Debug("IdentityResources being populated");
                    foreach (var resource in Config.IdentityResources.ToList())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                else
                {
                    Log.Debug("IdentityResources already populated");
                }

                if (!context.ApiScopes.Any())
                {
                    Log.Debug("ApiScopes being populated");
                    foreach (var resource in Config.ApiScopes.ToList())
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                else
                {
                    Log.Debug("ApiScopes already populated");
                }

                if (!context.ApiResources.Any())
                {
                    Log.Debug("ApiResources being populated");
                    foreach (var resource in Config.ApiResources.ToList())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                else
                {
                    Log.Debug("ApiResources already populated");
                }
            }
        }
    }
}