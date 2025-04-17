using System;

namespace Lunafy.Services.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityType)
        : base($"Entity of type {entityType} not found.")
    { }
}