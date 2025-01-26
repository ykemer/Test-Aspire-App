﻿using Contracts.Students.Events;

using MassTransit;

namespace Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByCourse;

public class DeleteEnrollmentsByCourseHandler : IRequestHandler<DeleteEnrollmentsByCourseCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteEnrollmentsByCourseHandler> _logger;
  private readonly IPublishEndpoint _publishEndpoint;

  public DeleteEnrollmentsByCourseHandler(ApplicationDbContext dbContext,
    ILogger<DeleteEnrollmentsByCourseHandler> logger, IPublishEndpoint publishEndpoint)
  {
    _dbContext = dbContext;
    _logger = logger;
    _publishEndpoint = publishEndpoint;
  }

  public async Task<ErrorOr<Deleted>> Handle(DeleteEnrollmentsByCourseCommand request,
    CancellationToken cancellationToken)
  {
    var events = await _dbContext.Enrollments
      .Where(enrollment => enrollment.CourseId == request.CourseId)
      .Select(enrollment => new DecreaseStudentEnrollmentsEvent { StudentId = enrollment.StudentId })
      .ToListAsync(cancellationToken);

    _dbContext.RemoveRange(_dbContext.Enrollments.Where(enrollment => enrollment.CourseId == request.CourseId));
    await _dbContext.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("Deleting enrollments for course {CourseId}", request.CourseId);
    if (events.Count > 0)
    {
      await _publishEndpoint.PublishBatch(events, cancellationToken);
    }

    return Result.Deleted;
  }
}
