﻿using Contracts.Common;

using ErrorOr;

using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;

using Service.Courses.Database;
using Service.Courses.Entities;
using Service.Courses.Features.Courses.ListCourses;

namespace Courses.Application.Handlers;

public class ListCoursesHandlerTests
{
  private ApplicationDbContext _dbContext;
  private ListCoursesHandler _handler;

  [SetUp]
  public void Setup()
  {
    DbContextOptions<ApplicationDbContext>? options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase("TestDatabase")
      .Options;
    _dbContext = new ApplicationDbContext(options);

    _handler = new ListCoursesHandler(_dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private async Task AddCoursesToDatabase(params Course[] courses)
  {
    _dbContext.Courses.RemoveRange(_dbContext.Courses);
    await _dbContext.Courses.AddRangeAsync(courses);
    await _dbContext.SaveChangesAsync();
  }

  [Test]
  public async Task Handle_ShouldReturnPagedList_WhenCoursesExist()
  {
    // Arrange
    Course[]? courses = Builder<Course>.CreateListOfSize(3).Build().ToArray();
    await AddCoursesToDatabase(courses);

    ListCoursesRequest? request = new() { PageNumber = 1, PageSize = 2 };

    // Act
    var result = await _handler.Handle(request, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items.Count, Is.EqualTo(2));
    Assert.That(result.Value.TotalCount, Is.EqualTo(3));
  }

  [Test]
  public async Task Handle_ShouldReturnPagedList_WhenRequestingSecondPage()
  {
    // Arrange
    Course[]? courses = Builder<Course>.CreateListOfSize(3).Build().ToArray();
    await AddCoursesToDatabase(courses);

    ListCoursesRequest? request = new() { PageNumber = 2, PageSize = 2 };

    // Act
    var result = await _handler.Handle(request, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items.Count, Is.EqualTo(1));
    Assert.That(result.Value.TotalCount, Is.EqualTo(3));
  }

  [Test]
  public async Task Handle_ShouldReturnFilteredPagedList_WhenQueryIsProvided()
  {
    // Arrange
    Course[]? courses = Builder<Course>.CreateListOfSize(3)
      .TheFirst(1).With(c => c.Name = "Course 1").And(c => c.Description = "Description 1")
      .TheNext(1).With(c => c.Name = "Special Course").And(c => c.Description = "Special Description")
      .TheNext(1).With(c => c.Name = "Course 3").And(c => c.Description = "Description 3")
      .Build().ToArray();
    await AddCoursesToDatabase(courses);

    ListCoursesRequest? request = new() { Query = "Special", PageNumber = 1, PageSize = 2 };

    // Act
    var result = await _handler.Handle(request, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items.Count, Is.EqualTo(1));
    Assert.That(result.Value.Items.First().Name, Is.EqualTo("Special Course"));
  }
}
