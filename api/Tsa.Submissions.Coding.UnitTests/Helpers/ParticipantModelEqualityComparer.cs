using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class ParticipantModelEqualityComparer : IEqualityComparer<ParticipantModel?>, IEqualityComparer<IList<ParticipantModel>?>
{
    public bool Equals(ParticipantModel? x, ParticipantModel? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;

        var participantNumbersMatch = x.ParticipantNumber == y.ParticipantNumber;
        var schoolNumbersMatch = x.SchoolNumber == y.SchoolNumber;

        return participantNumbersMatch &&
               schoolNumbersMatch;
    }

    public bool Equals(IList<ParticipantModel>? x, IList<ParticipantModel>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftParticipant in x)
        {
            var rightParticipant = y.SingleOrDefault(participant => participant.ParticipantId == leftParticipant.ParticipantId);

            if (!Equals(leftParticipant, rightParticipant)) return false;
        }

        return true;
    }

    public int GetHashCode(ParticipantModel? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }

    public int GetHashCode(IList<ParticipantModel>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
