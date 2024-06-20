﻿namespace Aspire_App.ApiService.Application.Students.Responses;

public class StudentResponse
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public DateTime DateOfBirth { get; set; }
}