using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class Team
{
    public CompetitionLevel? CompetitionLevel { get; set; }

    public string? SchoolNumber { get; set; }

    public string? TeamNumber { get; set; }
}
