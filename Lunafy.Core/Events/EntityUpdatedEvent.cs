using Lunafy.Core.Domains;

namespace Lunafy.Data;

public partial class EntityUpdatedEvent<T> where T : BaseEntity
{
    public EntityUpdatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}