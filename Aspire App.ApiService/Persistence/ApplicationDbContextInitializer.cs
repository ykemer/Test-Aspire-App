﻿using Aspire_App.ApiService.Features.Auth;
using Aspire_App.ApiService.Features.Courses;
using Aspire_App.ApiService.Features.Enrollments;
using Aspire_App.ApiService.Features.Students;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Aspire_App.ApiService.Persistence;

public sealed class ApplicationDbContextInitializer : IApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsNpgsql())
            {
                try
                {
                    _context.Database.EnsureCreated();
                    await _context.Database.MigrateAsync();
                }
                catch
                {
                    
                }
            }
            else if (_context.Database.IsSqlServer())
            {
                var dbCreator = _context.GetService<IRelationalDatabaseCreator>();
                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    if (!await dbCreator.ExistsAsync())
                        try
                        {
                            await dbCreator.CreateAsync();
                            await _context.Database.MigrateAsync();
                        }
                        catch
                        {
                        }
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole("Administrator");
        var userRole = new IdentityRole("User");

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
            await _roleManager.CreateAsync(userRole);
        }


        // Default users
        var administrator = new ApplicationUser
        {
            UserName = "admin@localhost",
            Email = "admin@localhost",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.Now.AddYears(-30),
            EmailConfirmed = true
        };

        var student = new ApplicationUser
        {
            UserName = "student@localhost",
            Email = "student@localhost",
            FirstName = "Marry",
            LastName = "Doe",
            DateOfBirth = DateTime.Now.AddYears(-25),
            EmailConfirmed = true
        };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, Environment.GetEnvironmentVariable("ADMIN_USER_PASSWORD")!);
            await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name }!);

            await _userManager.CreateAsync(student, Environment.GetEnvironmentVariable("ADMIN_USER_PASSWORD")!);
            await _userManager.AddToRolesAsync(student, new[] { userRole.Name }!);
        }

        if (!_context.Students.Any())
        {
            _context.Students.Add(new Student
            {
                Id = Guid.Parse(student.Id),
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            });

            await _context.SaveChangesAsync();
        }

        if (!_context.Courses.Any())
        {
            var math = new Course
            {
                Name = "Math",
                Description = "Math course"
            };

            _context.Courses.Add(math);

            _context.Courses.Add(new Course
            {
                Name = "Physics",
                Description = "Physics course"
            });

            _context.Courses.Add(new Course
            {
                Name = "Coding",
                Description = "Coding course"
            });

            _context.StudentEnrollments.Add(new StudentEnrollment
            {
                StudentId = Guid.Parse(student.Id),
                CourseId = math.Id
            });
        }


        await _context.SaveChangesAsync();
    }
}