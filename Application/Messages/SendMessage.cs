using ChatServer.Domain.Messages;

namespace ChatServer.Application.Messages;

public record SendMessageCommand(string Text, int UserId);
public record SendMessageResult(int Id, string Text, DateTime SentAt);

public class SendMessageHandler
{
    private readonly IMessageRepository _messageRepository;

    public SendMessageHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<SendMessageResult> Handle(SendMessageCommand command)
    {
        var message = Message.Create(command.Text, command.UserId);
        await _messageRepository.AddAsync(message);
        return new SendMessageResult(message.Id, message.Text, message.SentAt);
    }
}
