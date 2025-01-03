﻿using System.ComponentModel.DataAnnotations;

namespace Service.Students.Entitites;

public class Student
{
    [Key] public string Id { get; init; } = Guid.NewGuid().ToString();

    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }

    public DateTime DateOfBirth { get; init; }

    public int EnrolledCourses { get; set; } = 0;
}