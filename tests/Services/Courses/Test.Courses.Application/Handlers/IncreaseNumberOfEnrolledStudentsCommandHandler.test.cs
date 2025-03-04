﻿using Courses.Application.Setup;

using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Service.Courses.Database;
using Service.Courses.Entities;
using Service.Courses.Features.Courses.IncreaseNumberOfEnrolledStudents;

namespace Courses.Application.Handlers;

public class IncreaseNumberOfEnrolledStudentsCommandHandlerTests
{
  private ApplicationDbContext _dbContext;
  private IncreaseNumberOfEnrolledStudentsCommandHandler _commandHandler;
  private Mock<ILogger<IncreaseNumberOfEnrolledStudentsCommandHandler>> _loggerMock;

  [SetUp]
  public void Setup()
  {
    _loggerMock = new Mock<ILogger<IncreaseNumberOfEnrolledStudentsCommandHandler>>();
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _commandHandler = new IncreaseNumberOfEnrolledStudentsCommandHandler(_loggerMock.Object, _dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    // Arrange
    var command = new IncreaseNumberOfEnrolledStudentsCommand("bad-id");

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code,
      Is.EqualTo("course_service.increase_number_of_enrolled_students.course.not_found"));
  }

  [Test]
  public async Task Handle_ShouldIncreaseEnrollmentsCount_WhenCourseExists()
  {
    // Arrange
    var course = Builder<Course>
      .CreateNew()
      .With(course => course.EnrollmentsCount, 5)
      .Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.SaveChangesAsync();

    var command = new IncreaseNumberOfEnrolledStudentsCommand(course.Id);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(course.EnrollmentsCount, Is.EqualTo(6));
  }
}
