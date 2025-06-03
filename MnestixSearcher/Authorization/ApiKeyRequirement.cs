using Microsoft.AspNetCore.Authorization;

namespace MnestixSearcher.Authorization
{
    public class ApiKeyRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ApiKeyRequirement()
        {
        }
    }

}
