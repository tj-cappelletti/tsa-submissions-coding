using System;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class LoginResponseModel
{
    public string Token { get; set; }

    public DateTime Expiration { get; set; }

    public LoginResponseModel(string token, DateTime expiration)
    {
        Token = token;
        Expiration = expiration;
    }
}
