﻿namespace ApiServices.Models.Requests;

public class RegisterRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}