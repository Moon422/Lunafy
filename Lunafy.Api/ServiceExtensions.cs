using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Services.Events;
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

    public static IServiceCollection RegisterEventConsumers(this IServiceCollection services)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        var assemblies = new List<Assembly> { currentAssembly };
        assemblies.AddRange(currentAssembly.GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Where(a => a != null));

        var consumerInterfaceType = typeof(IConsumer<>);

        bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                return type.FindInterfaces((_, _) => true, null)
                    .Where(implementedInterface => implementedInterface.IsGenericType).Any(implementedInterface =>
                        genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition()));
            }
            catch
            {
                return false;
            }
        }

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                if (!consumerInterfaceType.IsAssignableFrom(type) &&
                    (!consumerInterfaceType.IsGenericTypeDefinition ||
                    DoesTypeImplementOpenGeneric(consumerInterfaceType, consumerInterfaceType)))
                    continue;

                if (type.IsInterface || type.IsAbstract)
                    continue;

                foreach (var findInterface in type.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>)))
                    services.AddScoped(findInterface, type);
            }
        }

        return services;
    }
}