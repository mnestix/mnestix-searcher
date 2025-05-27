using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MnestixSearcher.ApiServices.Settings;

namespace MnestixSearcher.Authorization
{
    public class ApiKeyRequirementHandler : AuthorizationHandler<ApiKeyRequirement>
    {
        private const string ApiKeyHeaderName = "X-API-KEY";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationSettings _apiKeySettings;

        public ApiKeyRequirementHandler(
            IHttpContextAccessor httpContextAccessor,
            IOptions<AuthenticationSettings> apiKeySettings)
        {
            _apiKeySettings = apiKeySettings.Value ??
                throw new ArgumentNullException(nameof(apiKeySettings));
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            SucceedRequirementIfApiKeyPresentAndValid(context, requirement);
            return Task.CompletedTask;
        }

        private void SucceedRequirementIfApiKeyPresentAndValid(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            var apiKey = new StringValues();
            _httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(ApiKeyHeaderName, out apiKey);

            if (apiKey == _apiKeySettings.ApiKey)
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail(new AuthorizationFailureReason(this,
                "For all methods except 'GET' you need a valid ApiKey in your header."));

        }
    }
}
