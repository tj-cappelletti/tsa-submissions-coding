using System;
using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.Contracts.Users;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static ParticipantResponse ToResponse(this Participant participant)
    {
        if (string.IsNullOrWhiteSpace(participant.IndividualNumber)) throw new InvalidOperationException("Participant number cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(participant.SchoolNumber))
            throw new InvalidOperationException("Participant School Number cannot be null or empty.");

        return new ParticipantResponse(
            participant.IndividualNumber,
            participant.SchoolNumber);
    }

    public static IEnumerable<ParticipantResponse> ToResponses(this IEnumerable<Participant> participants)
    {
        return participants.Select(participant => participant.ToResponse());
    }
}
