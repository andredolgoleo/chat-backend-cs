using ChatServer.Domain.Users;
using ChatServer.Domain.Messages;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
}
