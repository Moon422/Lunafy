using Lunafy.Core.Domains;

namespace Lunafy.Data;

public partial class EntityInsertedEvent<T> where T : BaseEntity
{
    public EntityInsertedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
