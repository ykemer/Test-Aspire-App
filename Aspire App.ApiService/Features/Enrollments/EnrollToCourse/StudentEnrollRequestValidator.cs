﻿using Contracts.Courses.Requests;
using FastEndpoints;
using FluentValidation;

namespace Aspire_App.ApiService.Features.Enrollments.EnrollToCourse;

public class StudentEnrollRequestValidator : Validator<ChangeCourseEnrollmentRequest>
{
    public StudentEnrollRequestValidator()
    {
        RuleFor(course => course.CourseId).NotEmpty().WithMessage("Course id can not be empty");
    }
}