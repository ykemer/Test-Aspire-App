// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Service.Courses.Entities;

namespace Service.Courses.Features.Courses;

public class CoursesDTO: Course
{
  public float Rank { get; set; } = 0;
}
