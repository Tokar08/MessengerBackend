using System.Text;
using System.Text.Json;

namespace MessangerBackend.Middlewares;

public class MessageFilteringMiddleware
{
    private static readonly List<string> ForbiddenWords = new()
    {
        "war",
        "russian",
        "russia",
        "conflict",
        "hate",
        "propaganda",
        "aggression",
        "dictatorship",
        "death",
        "terrorism",
        "murder"
    };

    private readonly RequestDelegate _next;

    public MessageFilteringMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            var isPostRequest = context.Request.Method == HttpMethods.Post;
            var isMessagePath = context.Request.Path.StartsWithSegments("/message");

            if (isPostRequest && isMessagePath)
            {
                context.Request.EnableBuffering();
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(requestBody))
                    throw new BadHttpRequestException("Request body is empty!", StatusCodes.Status400BadRequest);

                var rootElement = JsonDocument.Parse(requestBody).RootElement;

                if (rootElement.TryGetProperty("text", out var textElement))
                {
                    var originalText = textElement.GetString()!;

                    var modifiedJson = JsonSerializer.Serialize(new
                    {
                        senderId = rootElement.GetProperty("senderId").GetInt32(),
                        chatId = rootElement.GetProperty("chatId").GetInt32(),
                        text = ReplaceForbiddenWords(originalText)
                    });

                    context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(modifiedJson));
                    context.Request.Body.Position = 0;
                }
            }

            await _next.Invoke(context);
        }
        catch (BadHttpRequestException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(ex.Message);
        }
    }

    private static string ReplaceForbiddenWords(string inputText)
    {
        ForbiddenWords.ForEach(forbiddenWord =>
        {
            var replacement = new string('*', forbiddenWord.Length);
            inputText = inputText.Replace(forbiddenWord, replacement, StringComparison.OrdinalIgnoreCase);
        });
        return inputText;
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseMessageFiltering(this IApplicationBuilder app)
    {
        return app.UseMiddleware<MessageFilteringMiddleware>();
    }

    public static IApplicationBuilder UseSearchStatistics(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SearchStatisticsMiddleware>();
    }
}