using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class BearerTokenMiddleware
{
    private readonly RequestDelegate _next;

    public BearerTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && !authHeader.StartsWith("Bearer "))
        {
            context.Request.Headers["Authorization"] = "Bearer " + authHeader;
        }

        await _next(context);
    }
}