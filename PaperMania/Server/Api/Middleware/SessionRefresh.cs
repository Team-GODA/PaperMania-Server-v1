﻿using Server.Application.Port;

namespace Server.Api.Middleware;

public class SessionRefresh
{
    private readonly RequestDelegate _next;

    public SessionRefresh(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";
        var method = context.Request.Method;

        if (!(path.Equals("/api/v1/auth/logout", StringComparison.OrdinalIgnoreCase) && method == "POST"))
        {
            if (context.Request.Headers.TryGetValue("Session-Id", out var sessionIds))
            {
                var sessionId = sessionIds.FirstOrDefault();
                if (!string.IsNullOrEmpty(sessionId))
                {
                    var sessionService = context.RequestServices.GetRequiredService<ISessionService>();

                    bool valid = await sessionService.ValidateSessionAsync(sessionId);
                    if (valid)
                    {
                        await sessionService.RefreshSessionAsync(sessionId);
                    }
                }
            }
        }
        await _next(context);
    }
}