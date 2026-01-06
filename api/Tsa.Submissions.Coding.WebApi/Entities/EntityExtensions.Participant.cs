using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    private static List<ParticipantModel> ParticipantsToParticipantModels(IEnumerable<Participant> participants)
    {
        return participants.Select(participant => participant.ToModel()).ToList();
    }

    public static ParticipantModel ToModel(this Participant participant)
    {
        return new ParticipantModel
        {
            ParticipantNumber = participant.ParticipantNumber,
            SchoolNumber = participant.SchoolNumber
        };
    }

    public static List<ParticipantModel> ToModels(this IList<Participant> participants)
    {
        return ParticipantsToParticipantModels(participants);
    }

    public static List<ParticipantModel> ToModels(this IEnumerable<Participant> participants)
    {
        return ParticipantsToParticipantModels(participants);
    }
}
