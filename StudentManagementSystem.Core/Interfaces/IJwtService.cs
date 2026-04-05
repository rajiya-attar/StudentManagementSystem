namespace StudentManagementSystem.Core.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string email, string userId);
    }
}
