namespace Tsa.Submissions.Coding.WebApi.Models;

public enum ErrorCodes
{
    Unauthorized = 1000,
    RequiredEntityNotFound = 2000,
    UnexpectedNullValue = 3000,
    UnexpectedMissingResource = 4000,
    EntityNotFound = 5000,
    EntityAlreadyExists = 6000,
    InvalidId = 7000
}