using System.Threading.Tasks;

namespace Lunafy.Services.Events;

public partial interface IConsumer<T>
{
    Task HandleEventAsync(T eventMessage);
}