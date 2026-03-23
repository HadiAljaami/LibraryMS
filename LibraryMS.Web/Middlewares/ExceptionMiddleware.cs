using System.Net;
using System.Text.Json;

namespace LibraryMS.Web.Middlewares;

public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode  =
            (int)HttpStatusCode.InternalServerError;

        // إذا كان طلب AJAX
        if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            var response = new
            {
                error   = "حدث خطأ في الخادم",
                details = ex.Message
            };
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
            return;
        }

        // إذا كان طلب عادي — redirect لصفحة الخطأ
        context.Response.Redirect("/Home/Error");
    }
}