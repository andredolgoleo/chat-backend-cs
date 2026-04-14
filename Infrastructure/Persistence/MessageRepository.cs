using ChatServer.Domain.Messages;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Infrastructure.Persistence;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Message>> GetAllAsync()
    {
        return await _context.Messages
            .Include(m => m.User)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task AddAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }
}