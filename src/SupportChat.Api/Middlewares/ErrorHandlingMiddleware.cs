using SupportChat.Application.Commands.Exceptions;
using System.Text.Json;

namespace SupportChat.Api.Middlewares;

public class ErrorHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ErrorHandlingMiddleware> _logger;

	public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task Invoke(HttpContext ctx)
	{
		try
		{
			await _next(ctx);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unhandled exception occurred while processing the request.");
			await HandleExceptionAsync(ctx, ex);
		}
	}

	private static Task HandleExceptionAsync(HttpContext ctx, Exception ex)
	{
		ctx.Response.ContentType = "application/json";

		ctx.Response.StatusCode = ex switch
		{
			QueueFullException _ => StatusCodes.Status429TooManyRequests,
			InvalidOperationException _ => StatusCodes.Status400BadRequest,
			_ => StatusCodes.Status500InternalServerError,
		};
		var result = JsonSerializer.Serialize(new
		{
			status = ctx.Response.StatusCode,
			reason = ex.Message
		});

		return ctx.Response.WriteAsync(result);
	}
}