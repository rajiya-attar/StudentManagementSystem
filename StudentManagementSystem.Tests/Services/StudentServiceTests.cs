using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using StudentManagementSystem.Core.DTOs;
using StudentManagementSystem.Core.Interfaces;
using StudentManagementSystem.Core.Models;
using StudentManagementSystem.Infrastructure.Services;

namespace StudentManagementSystem.Tests.Services
{
    public class StudentServiceTests
    {
        private readonly Mock<IStudentRepository> _studentRepositoryMock;
        private readonly Mock<ILogger<StudentService>> _loggerMock;
        private readonly StudentService _studentService;

        public StudentServiceTests()
        {
            _studentRepositoryMock = new Mock<IStudentRepository>();
            _loggerMock = new Mock<ILogger<StudentService>>();
            _studentService = new StudentService(_studentRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllStudentsAsync_ShouldReturnAllStudents()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student { Id = 1, Name = "John Doe", Email = "john@example.com", Age = 22, Course = "CS" },
                new Student { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Age = 21, Course = "Math" }
            };

            _studentRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(students);

            // Act
            var result = await _studentService.GetAllStudentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _studentRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ExistingStudent_ShouldReturnStudent()
        {
            // Arrange
            var studentId = 1;
            var student = new Student { Id = studentId, Name = "John Doe", Email = "john@example.com", Age = 22, Course = "CS" };

            _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(studentId))
                .ReturnsAsync(student);

            // Act
            var result = await _studentService.GetStudentByIdAsync(studentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(studentId, result.Id);
            Assert.Equal("John Doe", result.Name);
            _studentRepositoryMock.Verify(repo => repo.GetByIdAsync(studentId), Times.Once);
        }

        [Fact]
        public async Task GetStudentByIdAsync_NonExistingStudent_ShouldReturnNull()
        {
            // Arrange
            var studentId = 999;

            _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(studentId))
                .ReturnsAsync((Student?)null);

            // Act
            var result = await _studentService.GetStudentByIdAsync(studentId);

            // Assert
            Assert.Null(result);
            _studentRepositoryMock.Verify(repo => repo.GetByIdAsync(studentId), Times.Once);
        }

        [Fact]
        public async Task CreateStudentAsync_ValidData_ShouldCreateStudent()
        {
            // Arrange
            var createDto = new CreateStudentDto
            {
                Name = "John Doe",
                Email = "john@example.com",
                Age = 22,
                Course = "Computer Science"
            };

            var createdStudent = new Student
            {
                Id = 1,
                Name = createDto.Name,
                Email = createDto.Email,
                Age = createDto.Age,
                Course = createDto.Course,
                CreatedDate = DateTime.UtcNow
            };

            _studentRepositoryMock.Setup(repo => repo.EmailExistsAsync(createDto.Email, null))
                .ReturnsAsync(false);
            _studentRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Student>()))
                .ReturnsAsync(createdStudent);

            // Act
            var result = await _studentService.CreateStudentAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(createDto.Name, result.Name);
            Assert.Equal(createDto.Email, result.Email);
            _studentRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Student>()), Times.Once);
        }

        [Fact]
        public async Task CreateStudentAsync_DuplicateEmail_ShouldThrowException()
        {
            // Arrange
            var createDto = new CreateStudentDto
            {
                Name = "John Doe",
                Email = "existing@example.com",
                Age = 22,
                Course = "Computer Science"
            };

            _studentRepositoryMock.Setup(repo => repo.EmailExistsAsync(createDto.Email, null))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _studentService.CreateStudentAsync(createDto));
            Assert.Contains("already exists", exception.Message);
        }

        [Fact]
        public async Task UpdateStudentAsync_ExistingStudent_ShouldUpdateStudent()
        {
            // Arrange
            var studentId = 1;
            var updateDto = new UpdateStudentDto
            {
                Name = "Updated Name",
                Email = "updated@example.com",
                Age = 23,
                Course = "Updated Course"
            };

            var existingStudent = new Student
            {
                Id = studentId,
                Name = updateDto.Name,
                Email = updateDto.Email,
                Age = updateDto.Age,
                Course = updateDto.Course
            };

            _studentRepositoryMock.Setup(repo => repo.ExistsAsync(studentId))
                .ReturnsAsync(true);
            _studentRepositoryMock.Setup(repo => repo.EmailExistsAsync(updateDto.Email, studentId))
                .ReturnsAsync(false);
            _studentRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Student>()))
                .ReturnsAsync(existingStudent);

            // Act
            var result = await _studentService.UpdateStudentAsync(studentId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.Name, result.Name);
            _studentRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Student>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStudentAsync_NonExistingStudent_ShouldReturnNull()
        {
            // Arrange
            var studentId = 999;
            var updateDto = new UpdateStudentDto
            {
                Name = "Updated Name",
                Email = "updated@example.com",
                Age = 23,
                Course = "Updated Course"
            };

            _studentRepositoryMock.Setup(repo => repo.ExistsAsync(studentId))
                .ReturnsAsync(false);

            // Act
            var result = await _studentService.UpdateStudentAsync(studentId, updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteStudentAsync_ExistingStudent_ShouldReturnTrue()
        {
            // Arrange
            var studentId = 1;
            _studentRepositoryMock.Setup(repo => repo.DeleteAsync(studentId))
                .ReturnsAsync(true);

            // Act
            var result = await _studentService.DeleteStudentAsync(studentId);

            // Assert
            Assert.True(result);
            _studentRepositoryMock.Verify(repo => repo.DeleteAsync(studentId), Times.Once);
        }

        [Fact]
        public async Task DeleteStudentAsync_NonExistingStudent_ShouldReturnFalse()
        {
            // Arrange
            var studentId = 999;
            _studentRepositoryMock.Setup(repo => repo.DeleteAsync(studentId))
                .ReturnsAsync(false);

            // Act
            var result = await _studentService.DeleteStudentAsync(studentId);

            // Assert
            Assert.False(result);
        }
    }
}
