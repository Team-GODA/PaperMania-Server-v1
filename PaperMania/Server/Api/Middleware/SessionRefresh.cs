using Server.Infrastructure.Service.Interface;

namespace Server.Api.Middleware;

public class SessionRefresh
{
    private readonly RequestDelegate _next;
    private readonly ISessionService _sessionService;

    public SessionRefresh(RequestDelegate next, ISessionService sessionService)
    {
        _next = next;
        _sessionService = sessionService;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Session-Id", out var sessionIds))
        {
            var sessionId = sessionIds.FirstOrDefault();
            if (!string.IsNullOrEmpty(sessionId))
            {
                bool valid = await _sessionService.ValidateSessionAsync(sessionId);
                if (valid)
                {
                    await _sessionService.RefreshSessionAsync(sessionId);
                }
            }
        }

        await _next(context);
    }
}