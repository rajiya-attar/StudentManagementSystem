using Microsoft.Extensions.Logging;
using StudentManagementSystem.Core.DTOs;
using StudentManagementSystem.Core.Interfaces;
using StudentManagementSystem.Core.Models;

namespace StudentManagementSystem.Infrastructure.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<StudentService> _logger;

        public StudentService(IStudentRepository studentRepository, ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            _logger.LogInformation("Getting all students");
            var students = await _studentRepository.GetAllAsync();
            return students.Select(MapToDto);
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            _logger.LogInformation("Getting student by ID: {StudentId}", id);
            var student = await _studentRepository.GetByIdAsync(id);
            return student == null ? null : MapToDto(student);
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto)
        {
            _logger.LogInformation("Creating new student: {StudentName}", createStudentDto.Name);

            // Check if email already exists
            if (await _studentRepository.EmailExistsAsync(createStudentDto.Email))
            {
                throw new InvalidOperationException($"Student with email '{createStudentDto.Email}' already exists");
            }

            var student = new Student
            {
                Name = createStudentDto.Name,
                Email = createStudentDto.Email,
                Age = createStudentDto.Age,
                Course = createStudentDto.Course
            };

            var createdStudent = await _studentRepository.CreateAsync(student);
            _logger.LogInformation("Student created successfully with ID: {StudentId}", createdStudent.Id);

            return MapToDto(createdStudent);
        }

        public async Task<StudentDto?> UpdateStudentAsync(int id, UpdateStudentDto updateStudentDto)
        {
            _logger.LogInformation("Updating student with ID: {StudentId}", id);

            // Check if student exists
            if (!await _studentRepository.ExistsAsync(id))
            {
                _logger.LogWarning("Student with ID: {StudentId} not found for update", id);
                return null;
            }

            // Check if email already exists (excluding current student)
            if (await _studentRepository.EmailExistsAsync(updateStudentDto.Email, id))
            {
                throw new InvalidOperationException($"Student with email '{updateStudentDto.Email}' already exists");
            }

            var student = new Student
            {
                Id = id,
                Name = updateStudentDto.Name,
                Email = updateStudentDto.Email,
                Age = updateStudentDto.Age,
                Course = updateStudentDto.Course
            };

            var updatedStudent = await _studentRepository.UpdateAsync(student);

            if (updatedStudent == null)
            {
                return null;
            }

            _logger.LogInformation("Student updated successfully with ID: {StudentId}", id);
            return MapToDto(updatedStudent);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            _logger.LogInformation("Deleting student with ID: {StudentId}", id);
            var result = await _studentRepository.DeleteAsync(id);

            if (result)
            {
                _logger.LogInformation("Student deleted successfully with ID: {StudentId}", id);
            }

            return result;
        }

        private static StudentDto MapToDto(Student student)
        {
            return new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Age = student.Age,
                Course = student.Course,
                CreatedDate = student.CreatedDate
            };
        }
    }
}
