using Lunafy.Core.Domains;

namespace Lunafy.Data;

public partial class EntityDeletedEvent<T> where T : BaseEntity
{
    public EntityDeletedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
