using MessangerBackend.Core.Services;

namespace MessangerBackend.Middlewares;

public class SearchStatisticsMiddleware
{
    private readonly ILogger<SearchStatisticsMiddleware> _logger;
    private readonly RequestDelegate _next;
    private const string NicknameParam = "nickname";

    public SearchStatisticsMiddleware(RequestDelegate next, ILogger<SearchStatisticsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        var searchStatisticsService = ctx.RequestServices.GetRequiredService<SearchStatisticsService>();
        
        if (ctx.Request.Path.StartsWithSegments("/api/users/search", StringComparison.OrdinalIgnoreCase))
        {
            var searchTerm = ctx.Request.Query[NicknameParam].ToString();
            if (!string.IsNullOrWhiteSpace(searchTerm))
                searchStatisticsService.IncrementSearchStat(searchTerm);
        }

        await _next.Invoke(ctx);
        _logger.LogInformation($"Path: {ctx.Request.Path} {Environment.NewLine} Query: {ctx.Request.Query}");
    }

}

