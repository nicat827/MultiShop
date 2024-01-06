using Microsoft.AspNetCore.Mvc;
using MultiShop.Utilities;

namespace MultiShop.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (NotFoundException ex)
            {
                string path = Path.Combine("https://localhost:7187", "error", "index");
                Console.WriteLine(ex.Message);
                context.Response.Redirect($"{path}?mess={ex.Message}&code=404");
            }
            catch (Exception ex)
            {
                string path = Path.Combine("https://localhost:7187", "error", "index");
                Console.WriteLine(ex.Message);
                context.Response.Redirect($"{path}?mess={ex.Message}&code=500");
            }
        }
    }
}
