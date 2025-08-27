using System;

namespace Client.Models;

public class UserLoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }

    public UserLoginModel(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public UserLoginModel(){}
}
