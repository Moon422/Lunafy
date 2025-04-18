using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lunafy.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddDbContext<LunafyDbContext>(options =>
        {
            var connectionString = Configuration.GetConnectionString("mysql");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException();

            var version = ServerVersion.AutoDetect(connectionString) ??
                throw new InvalidOperationException();

            options.UseMySql(connectionString, version);
        });

        var currentAssembly = Assembly.GetExecutingAssembly();
        var assemblies = new List<Assembly> { currentAssembly };
        assemblies.AddRange(currentAssembly.GetReferencedAssemblies()
            .Select(name => Assembly.Load(name))
            .Where(a => a != null));

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttributes<TransientDependencyAttribute>().Any());

                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute<TransientDependencyAttribute>();
                    if (attribute is null)
                        continue;

                    services.AddTransient(attribute.ServiceType, type);
                }

                types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttributes<ScopeDependencyAttribute>().Any());

                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute<ScopeDependencyAttribute>();
                    if (attribute is null)
                        continue;

                    services.AddScoped(attribute.ServiceType, type);
                }

                types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttributes<SingletonDependencyAttribute>().Any());

                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute<SingletonDependencyAttribute>();
                    if (attribute is null)
                        continue;

                    services.AddSingleton(attribute.ServiceType, type);
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"Warning: Could not load types from assembly {assembly.GetName().Name}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scanning assembly {assembly.GetName().Name}: {ex.Message}");
            }
        }

        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Lunafy API",
                Version = "v1"
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lunafy API v1");
            });
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}