﻿using Service.Courses.Database.Entities;

namespace Service.Courses.Features.Courses;

public class CoursesDto : Course
{
  public float Rank { get; set; } = 0;
}

