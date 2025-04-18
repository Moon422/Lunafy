using System;

namespace Lunafy.Core.Infrastructure.Dependencies;

[AttributeUsage(AttributeTargets.Class)]
public class ScopeDependencyAttribute : Attribute
{
    public Type ServiceType { get; }

    public ScopeDependencyAttribute(Type serviceType)
    {
        ServiceType = serviceType;
    }
}
