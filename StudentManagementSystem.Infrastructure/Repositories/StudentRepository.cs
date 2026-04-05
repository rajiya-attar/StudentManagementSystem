using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentManagementSystem.Core.Interfaces;
using StudentManagementSystem.Core.Models;
using StudentManagementSystem.Infrastructure.Data;

namespace StudentManagementSystem.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(ApplicationDbContext context, ILogger<StudentRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all students from database");
            return await _context.Students
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving student with ID: {StudentId}", id);
            return await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
        }

        public async Task<Student> CreateAsync(Student student)
        {
            _logger.LogInformation("Creating new student: {StudentName}", student.Name);
            student.CreatedDate = DateTime.UtcNow;
            student.IsActive = true;

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return student;
        }

        public async Task<Student?> UpdateAsync(Student student)
        {
            _logger.LogInformation("Updating student with ID: {StudentId}", student.Id);

            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == student.Id && s.IsActive);

            if (existingStudent == null)
            {
                _logger.LogWarning("Student with ID: {StudentId} not found for update", student.Id);
                return null;
            }

            existingStudent.Name = student.Name;
            existingStudent.Email = student.Email;
            existingStudent.Age = student.Age;
            existingStudent.Course = student.Course;

            await _context.SaveChangesAsync();
            return existingStudent;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting student with ID: {StudentId}", id);

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (student == null)
            {
                _logger.LogWarning("Student with ID: {StudentId} not found for deletion", id);
                return false;
            }

            // Soft delete
            student.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Students
                .AnyAsync(s => s.Id == id && s.IsActive);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _context.Students
                .Where(s => s.Email == email && s.IsActive);

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
