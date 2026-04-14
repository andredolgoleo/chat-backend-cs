namespace ChatServer.Domain.Messages;

public interface IMessageRepository
{
    Task<List<Message>> GetAllAsync();
    Task AddAsync(Message message);
}