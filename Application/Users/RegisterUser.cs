using ChatServer.Domain.Users;

namespace ChatServer.Application.Users;

public record RegisterUserCommand(string Username, string Password);
public record RegisterUserResult(int Id, string Username);

public class RegisterUserHandler
{
    private readonly IUserRepository _userRepository;

    public RegisterUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand command)
    {
        var existing = await _userRepository.GetByUsernameAsync(command.Username);
        if (existing is not null)
            throw new InvalidOperationException("User with this username already exists");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);
        var user = User.Create(command.Username, passwordHash);

        await _userRepository.AddAsync(user);

        return new RegisterUserResult(user.Id, user.Username);
    }
}