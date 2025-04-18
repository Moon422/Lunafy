using System;

namespace Lunafy.Core.Infrastructure.Dependencies;

[AttributeUsage(AttributeTargets.Class)]
public class TransientDependencyAttribute : Attribute
{
    public Type ServiceType { get; }

    public TransientDependencyAttribute(Type serviceType)
    {
        ServiceType = serviceType;
    }
}
