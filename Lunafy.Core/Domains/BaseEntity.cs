using System;

namespace Lunafy.Core.Domains;

public abstract class BaseEntity
{
    public int Id { get; set; }
}

public interface ISoftDeleted
{
    public bool Deleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}

public interface ICreationLogged
{
    public DateTime CreatedOn { get; set; }
}

public interface IModificationLogged
{
    public DateTime? ModifiedOn { get; set; }
}
