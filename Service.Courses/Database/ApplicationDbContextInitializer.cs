﻿using Microsoft.EntityFrameworkCore;
using Service.Courses.Entities;

namespace Service.Courses.Database;

public sealed class ApplicationDbContextInitializer 
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
 
    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
      
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();
            await _context.Database.MigrateAsync();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,
                "An error occurred while trying to migrate the database. This is expected when using project locally."); 
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
        }
    }

    public async Task TrySeedAsync()
    {
        
        if (!await _context.Courses.AnyAsync())
        {
            _context.Courses.Add(new Course
            {
                Id = "0b9de47c-fc66-4fb5-befe-5569b0fd6dd0",
                Name = "Math",
                Description = "Math course",
                EnrollmentsCount = 1
            });

            _context.Courses.Add(new Course
            {
                Name = "Physics",
                Description = "Physics course",
                EnrollmentsCount = 0
            });

            _context.Courses.Add(new Course
            {
                Name = "Coding",
                Description = "Coding course",
                EnrollmentsCount = 0
            });
        }


        await _context.SaveChangesAsync();
    }
}