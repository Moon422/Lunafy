using System.Threading.Tasks;
using Lunafy.Core.Domains;

namespace Lunafy.Core.Events;

public partial interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event);

    void Publish<TEvent>(TEvent @event);
}
