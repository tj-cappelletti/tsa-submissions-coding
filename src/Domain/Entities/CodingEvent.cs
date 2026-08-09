using Tsa.Submissions.Coding.Domain.Common.Auditing;

namespace Tsa.Submissions.Coding.Domain.Entities
{
    public class CodingEvent : AuditableEntity
    {
        public CompetitionLevel CompetitionLevel { get; private set; }

        public string Name { get; private set; } = string.Empty;
        
        public string Description { get; private set; } = string.Empty;
        
        public DateTime StartDate { get; private set; }
        
        public DateTime EndDate { get; private set; }

        private CodingEvent() { } // for ORM

        public static CodingEvent Create(CompetitionLevel competitionLevel, string name, string description, DateTime startDate, DateTime endDate, User createdBy)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));

            if (startDate >= endDate) throw new ArgumentException("Start date must be before end date.", nameof(startDate));

            ArgumentNullException.ThrowIfNull(createdBy);

            var codingEvent = new CodingEvent
            {
                CompetitionLevel = competitionLevel,
                EndDate = endDate,
                Description = description.Trim(),
                Name = name.Trim(),
                StartDate = startDate,
            };
            
            codingEvent.SetCreatedAudit(createdBy.UserName, createdBy.Id);

            return codingEvent;
        }
    }
}
