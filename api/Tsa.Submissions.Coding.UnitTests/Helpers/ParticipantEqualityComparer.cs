using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class ParticipantEqualityComparer : IEqualityComparer<Participant?>, IEqualityComparer<IList<Participant>?>
{
    public bool Equals(Participant? x, Participant? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;

        var participantNumbersMatch = x.ParticipantNumber == y.ParticipantNumber;
        var schoolNumbersMatch = x.SchoolNumber == y.SchoolNumber;

        return participantNumbersMatch &&
               schoolNumbersMatch;
    }

    public bool Equals(IList<Participant>? x, IList<Participant>? y)
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

    public int GetHashCode(Participant? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }

    public int GetHashCode(IList<Participant>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
