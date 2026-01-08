using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Models;

public static partial class ModelExtensions
{
    public static List<Participant> ToEntities(this IList<ParticipantModel> participants)
    {
        return participants.Select(participant => participant.ToEntity()).ToList();
    }

    public static Participant ToEntity(this ParticipantModel participantModel)
    {
        return new Participant
        {
            ParticipantNumber = participantModel.ParticipantNumber,
            SchoolNumber = participantModel.SchoolNumber
        };
    }
}
