namespace ChatServer.Domain.Users;

public class User
{
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() { } // нужен для EF Core

    public static User Create(string username, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username не может быть пустым");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password не может быть пустым");

        return new User
        {
            Username = username,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
    }
}