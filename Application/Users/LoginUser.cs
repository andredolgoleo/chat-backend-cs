using ChatServer.Domain.Users;
using ChatServer.Infrastructure.Auth;

namespace ChatServer.Application.Users;

public record LoginUserCommand(string Username, string Password);
public record LoginUserResult(string Token);

public class LoginUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;

    public LoginUserHandler(IUserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginUserResult> Handle(LoginUserCommand command)
    {
        var user = await _userRepository.GetByUsernameAsync(command.Username);
        if (user is null)
            throw new InvalidOperationException("Invalid username or password");

        var isValid = BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash);
        if (!isValid)
            throw new InvalidOperationException("Invalid username or password");

        var token = _jwtService.GenerateToken(user);
        return new LoginUserResult(token);
    }
}