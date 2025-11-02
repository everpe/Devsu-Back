using System.Net;
using System.Text.Json;
using DevsuApp.BE.Application.Exceptions;

namespace DevsuApp.BE.API.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió una excepción: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Ocurrió un error interno en el servidor";
        var details = exception.Message;

        switch (exception)
        {
            case SaldoInsuficienteException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Saldo no disponible";
                break;

            case CupoDiarioExcedidoException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Cupo diario Excedido";
                break;

            case BusinessException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            case ArgumentException:
            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = "Recurso no encontrado";
                break;
        }

        var response = new
        {
            statusCode = (int)statusCode,
            message,
            details
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}