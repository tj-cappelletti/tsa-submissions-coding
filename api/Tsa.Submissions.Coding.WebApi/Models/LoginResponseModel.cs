using System;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class LoginResponseModel
{
    public string Token { get; set; }

    public DateTimeOffset Expiration { get; set; }

    public LoginResponseModel(string token, DateTimeOffset expiration)
    {
        Token = token;
        Expiration = expiration;
    }
}
