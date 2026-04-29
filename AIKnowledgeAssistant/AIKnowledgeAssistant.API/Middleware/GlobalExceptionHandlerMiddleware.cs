using AIKnowledgeAssistant.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeAssistant.API.Middleware
{
    internal sealed class GlobalExceptionHandlerMiddleware(RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occured");
                context.Response.StatusCode = ex switch
                {
                    ApplicationException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };
                await context.Response.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Type = ex.GetType().Name,
                        Title = "An error has occured",
                        Detail = ex.Message
                    });
            }
        }
    }
}
