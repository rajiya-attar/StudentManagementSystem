using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Core.DTOs;
using StudentManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace StudentManagementSystem.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<StudentDto>>>> GetAll()
        {
            _logger.LogInformation("Getting all students");
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(ApiResponse<IEnumerable<StudentDto>>.SuccessResponse(students, "Students retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StudentDto>>> GetById(int id)
        {
            _logger.LogInformation("Getting student with ID: {StudentId}", id);
            var student = await _studentService.GetStudentByIdAsync(id);

            if (student == null)
            {
                _logger.LogWarning("Student with ID: {StudentId} not found", id);
                return NotFound(ApiResponse<StudentDto>.ErrorResponse($"Student with ID {id} not found"));
            }

            return Ok(ApiResponse<StudentDto>.SuccessResponse(student, "Student retrieved successfully"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StudentDto>>> Create([FromBody] CreateStudentDto createStudentDto)
        {
            _logger.LogInformation("Creating new student: {StudentName}", createStudentDto.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<StudentDto>.ErrorResponse("Invalid data provided"));
            }

            var createdStudent = await _studentService.CreateStudentAsync(createStudentDto);
            return CreatedAtAction(nameof(GetById), new { id = createdStudent.Id },
                ApiResponse<StudentDto>.SuccessResponse(createdStudent, "Student created successfully", 201));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<StudentDto>>> Update(int id, [FromBody] UpdateStudentDto updateStudentDto)
        {
            _logger.LogInformation("Updating student with ID: {StudentId}", id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<StudentDto>.ErrorResponse("Invalid data provided"));
            }

            var updatedStudent = await _studentService.UpdateStudentAsync(id, updateStudentDto);

            if (updatedStudent == null)
            {
                _logger.LogWarning("Student with ID: {StudentId} not found for update", id);
                return NotFound(ApiResponse<StudentDto>.ErrorResponse($"Student with ID {id} not found"));
            }

            return Ok(ApiResponse<StudentDto>.SuccessResponse(updatedStudent, "Student updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            _logger.LogInformation("Deleting student with ID: {StudentId}", id);
            var result = await _studentService.DeleteStudentAsync(id);

            if (!result)
            {
                _logger.LogWarning("Student with ID: {StudentId} not found for deletion", id);
                return NotFound(ApiResponse<bool>.ErrorResponse($"Student with ID {id} not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Student deleted successfully"));
        }
    }
}
