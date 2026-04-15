using Tsa.Submissions.Coding.Contracts.Events;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static EventResponse ToResponse(this Event @event)
    {
        return new EventResponse(@event.IsActive, @event.StartTime, @event.EndTime);
    }
}
