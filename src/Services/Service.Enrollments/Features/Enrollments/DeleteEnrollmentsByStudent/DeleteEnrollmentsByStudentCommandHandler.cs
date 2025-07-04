﻿using Contracts.Courses.Events.ChangeCourseEnrollmentsCount;

using MassTransit;

namespace Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByStudent;

public class DeleteEnrollmentsByStudentCommandHandler : IRequestHandler<DeleteEnrollmentsByStudentCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteEnrollmentsByStudentCommandHandler> _logger;
  private readonly IPublishEndpoint _publishEndpoint;

  public DeleteEnrollmentsByStudentCommandHandler(ApplicationDbContext dbContext,
    ILogger<DeleteEnrollmentsByStudentCommandHandler> logger, IPublishEndpoint publishEndpoint)
  {
    _dbContext = dbContext;
    _logger = logger;
    _publishEndpoint = publishEndpoint;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(DeleteEnrollmentsByStudentCommand request,
    CancellationToken cancellationToken)
  {
    var events = await _dbContext.Enrollments
      .Where(enrollment => enrollment.CourseId == request.StudentId)
      .Select(enrollment => new ChangeCourseEnrollmentsCountEvent { CourseId = enrollment.CourseId , IsIncrease = false})
      .ToListAsync(cancellationToken);

    _dbContext.RemoveRange(_dbContext.Enrollments.Where(enrollment => enrollment.StudentId == request.StudentId));
    await _dbContext.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("Deleting enrollments for Student {StudentId}", request.StudentId);
    if (events.Count > 0)
    {
      await _publishEndpoint.PublishBatch(events, cancellationToken);
    }

    return Result.Deleted;
  }
}
