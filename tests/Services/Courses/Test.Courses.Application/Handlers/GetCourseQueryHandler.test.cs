﻿using Courses.Application.Setup;

using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Service.Courses.Database;
using Service.Courses.Entities;
using Service.Courses.Features.Courses.GetCourse;

namespace Courses.Application.Handlers;

public class GetCourseQueryHandlerTests
{
  private ApplicationDbContext _dbContext;
  private GetCourseQueryHandler _queryHandler;
  private Mock<ILogger<GetCourseQueryHandler>> _loggerMock;

  [SetUp]
  public void Setup()
  {
    _loggerMock = new Mock<ILogger<GetCourseQueryHandler>>();
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _queryHandler = new GetCourseQueryHandler(_loggerMock.Object, _dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnCourse_WhenCourseExists()
  {
    // Arrange
    var course = Builder<Course>
      .CreateNew()
      .With(c => c.Name = "Test Course")
      .Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.SaveChangesAsync();

    GetCourseQuery? query = new(course.Id);

    // Act
    var result = await _queryHandler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Id, Is.EqualTo(course.Id));
    Assert.That(result.Value.Name, Is.EqualTo("Test Course"));
  }

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    // Arrange
    GetCourseQuery? query = new("bad-id");

    // Act
    var result = await _queryHandler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("courses_service.get_course.not_found"));
  }
}
