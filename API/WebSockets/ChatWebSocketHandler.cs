using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ChatServer.Application.Messages;

namespace ChatServer.API.WebSockets;

public class ChatWebSocketHandler
{
    private static readonly ConcurrentDictionary<string, WebSocket> _connections = new();
    private readonly SendMessageHandler _sendMessageHandler;

    public ChatWebSocketHandler(SendMessageHandler sendMessageHandler)
    {
        _sendMessageHandler = sendMessageHandler;
    }

    public async Task HandleAsync(WebSocket socket, int userId, string username)
    {
        var connectionId = Guid.NewGuid().ToString();
        _connections.TryAdd(connectionId, socket);

        var buffer = new byte[1024 * 4];

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var incoming = JsonSerializer.Deserialize<IncomingMessage>(json);

                if (incoming is null) continue;

                // Сохраняем в БД
                var saved = await _sendMessageHandler.Handle(new SendMessageCommand(incoming.Text, userId));

                // Рассылаем всем подключённым
                var outgoing = JsonSerializer.Serialize(new OutgoingMessage(saved.Id, username, saved.Text, saved.SentAt));
                await BroadcastAsync(outgoing);
            }
        }
        finally
        {
            _connections.TryRemove(connectionId, out _);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
        }
    }

    private async Task BroadcastAsync(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        foreach (var (_, socket) in _connections)
        {
            if (socket.State == WebSocketState.Open)
                await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

public record IncomingMessage(string Text);
public record OutgoingMessage(int Id, string Username, string Text, DateTime SentAt);
