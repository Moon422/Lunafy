using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lunafy.Core.Infrastructure.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace Lunafy.Api;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
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

        return services;
    }
}