using Tsa.Submissions.Coding.Domain.Authorization;
using Tsa.Submissions.Coding.Domain.Common.Auditing;

namespace Tsa.Submissions.Coding.Domain.Entities;

public class User : AuditableEntity
{
    public string PasswordHash { get; private set; } = string.Empty;

    public string Role { get; private set; } = string.Empty;
    
    public string? TeamId { get; private set; }
    
    public string UserName { get; private set; } = string.Empty;

    private User() { } // for ORM

    public static User CreateJudge(string userName, string passwordHash, User createdBy)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Username is required.", nameof(userName));

        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        
        ArgumentNullException.ThrowIfNull(createdBy);

        var user = new User
        {
            UserName = userName.Trim(),
            PasswordHash = passwordHash,
            Role = UserRoles.Judge,
            TeamId = null
        };

        user.SetCreatedAudit(createdBy.UserName, createdBy.Id);
        
        return user;
    }

    public static User CreateParticipant(string userName, string passwordHash, string teamId, User createdBy)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Username is required.", nameof(userName));
        
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        
        if (string.IsNullOrWhiteSpace(teamId)) throw new ArgumentException("Team is required.", nameof(teamId));
        
        ArgumentNullException.ThrowIfNull(createdBy);

        var user = new User
        {
            UserName = userName.Trim(),
            PasswordHash = passwordHash,
            Role = UserRoles.Participant,
            TeamId = teamId.Trim()
        };

        user.SetCreatedAudit(createdBy.UserName, createdBy.Id);
        
        return user;
    }
}
