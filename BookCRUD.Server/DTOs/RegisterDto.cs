namespace BookCRUD.Server.DTOs;

public class RegisterDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } // e.g., "User", "Admin"
}
