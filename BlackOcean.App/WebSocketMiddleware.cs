using BlackOcean.Simulation;

namespace BlackOcean.App;

public class WebSocketMiddleware(Game game, GameService gameService, ILoggerFactory loggerFactory) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path != "/ws")
        {
            await next(context);
            return;
        }
        
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        
        var connection = new WebSocketConnection(webSocket, game, loggerFactory.CreateLogger<WebSocketConnection>()); 
        gameService.AddConnection(connection);
        await connection.StartAsync(context.RequestAborted);
        gameService.RemoveConnection(connection);
    }
}