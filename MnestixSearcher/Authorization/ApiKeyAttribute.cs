using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using MnestixSearcher.ApiServices.Settings;

namespace MnestixSearcher.Authorization
{
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string Apikeyname = "X-API-KEY";

        /// <inheritdoc/>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(Apikeyname, out var extractedApiKey))
            {
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    Content = "Api Key was not provided"
                };
                return;
            }

            var apiKeySettings = context.HttpContext.RequestServices
                .GetRequiredService<IOptions<AuthenticationSettings>>();

            if (!apiKeySettings.Value.ApiKey.Equals(extractedApiKey))
            {
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    Content = "Api Key is not valid"
                };
                return;
            }

            await next();
        }
    }
}
