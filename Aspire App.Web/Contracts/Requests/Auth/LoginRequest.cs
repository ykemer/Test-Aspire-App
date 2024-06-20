﻿using System.ComponentModel.DataAnnotations;

namespace Aspire_App.Web.Contracts.Requests.Auth;

public class LoginRequest
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required] public string Password { get; set; }
}