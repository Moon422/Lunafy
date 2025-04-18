using System;

namespace Lunafy.Core.Infrastructure.Dependencies;

[AttributeUsage(AttributeTargets.Class)]
public class SingletonDependencyAttribute : Attribute
{
    public Type ServiceType { get; }

    public SingletonDependencyAttribute(Type serviceType)
    {
        ServiceType = serviceType;
    }
}