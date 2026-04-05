using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Core.Models;

namespace StudentManagementSystem.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if we already have data
            if (context.Users.Any())
            {
                return; // Database already seeded
            }

            // Add initial students if empty
            if (!context.Students.Any())
            {
                var students = new Student[]
                {
                    new Student
                    {
                        Name = "John Doe",
                        Email = "john.doe@example.com",
                        Age = 22,
                        Course = "Computer Science",
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Student
                    {
                        Name = "Jane Smith",
                        Email = "jane.smith@example.com",
                        Age = 21,
                        Course = "Mathematics",
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    }
                };

                context.Students.AddRange(students);
                context.SaveChanges();
            }
        }
    }
}
