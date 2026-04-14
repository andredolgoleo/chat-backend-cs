using ChatServer.Domain.Users;

namespace ChatServer.Domain.Messages;

public class Message
{
    public int Id { get; private set; }
    public string Text { get; private set; }
    public DateTime SentAt { get; private set; }

    public int UserId { get; private set; }
    public User User { get; private set; }

    private Message() { } // нужен для EF Core

    public static Message Create(string text, int userId)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Текст сообщения не может быть пустым");

        return new Message
        {
            Text = text,
            UserId = userId,
            SentAt = DateTime.UtcNow
        };
    }
}