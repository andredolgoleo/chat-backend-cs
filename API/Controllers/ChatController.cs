using System.Net.WebSockets;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using ChatServer.API.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ChatServer.API.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly ChatWebSocketHandler _chatHandler;

    public ChatController(ChatWebSocketHandler chatHandler)
    {
        _chatHandler = chatHandler;
    }

    [HttpGet("ws")]
    public async Task Connect([FromQuery] string token)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var principal = ValidateToken(token);
        if (principal is null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var username = principal.FindFirst(ClaimTypes.Name)!.Value;

        var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        await _chatHandler.HandleAsync(socket, userId, username);
    }

    private System.Security.Claims.ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var secret = Environment.GetEnvironmentVariable("JWT_SECRET")!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out _);
        }
        catch
        {
            return null;
        }
    }
}