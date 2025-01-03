﻿using Contracts.Common;
using Service.Students.Entitites;

namespace Service.Students.Features.ListStudent;

public class ListStudentsHandler: IRequestHandler<ListStudentsQuery, ErrorOr<PagedList<Student>>>
{
    private readonly ApplicationDbContext _dbContext;

    public ListStudentsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ErrorOr<PagedList<Student>>> Handle(ListStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = _dbContext.Students.AsQueryable();
        var pagedStudents = PagedList<Student>.Create(students, request.PageNumber, request.PageSize);
        return Task.FromResult<ErrorOr<PagedList<Student>>>(pagedStudents); 
    }
}