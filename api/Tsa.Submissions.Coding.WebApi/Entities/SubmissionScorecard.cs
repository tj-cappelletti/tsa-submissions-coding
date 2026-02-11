using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class SubmissionScorecard
{
    public int BaselineCyclomaticComplexity { get; set; }

    public int BaselineLinesOfCode { get; set; }

    public long BaselineTimeMs { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal CodeQualityScore { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal CorrectnessScore { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal CyclomaticComplexityScore { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal EfficiencyScore { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal FinalScore { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal LinesOfCodeScore { get; set; }

    public int ParticipantCyclomaticComplexity { get; set; }

    public int ParticipantLinesOfCode { get; set; }

    public long ParticipantTimeMs { get; set; }

    public int PassedTestCases { get; set; }

    public int RoundedToDecimalPlaces { get; set; } = 3;

    public string RoundingMode { get; set; } = "AwayFromZero";

    public int ScoringVersion { get; set; } = 1;

    public int TotalTestCases { get; set; }
}
