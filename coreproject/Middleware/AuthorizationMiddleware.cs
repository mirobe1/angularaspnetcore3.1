using coreproject.Model;
using coreproject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using WebApi.Helpers;

namespace coreproject.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEncryptonService _encryptionService;

        public AuthorizationMiddleware(RequestDelegate next, IEncryptonService encryptionService)
        {
            _next = next;
            _encryptionService = encryptionService;
        }

        public async Task Invoke(HttpContext context, ApplicationContext dataContext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault();

            if (token != null)
                await attachAccountToContext(context, dataContext, token);

            await _next(context);
        }

        private async Task attachAccountToContext(HttpContext context, ApplicationContext dataContext, string token)
        {
            try
            {
                var accountId = _encryptionService.DecryptToken(token).Split("_").First();
                var token2 = _encryptionService.DecryptToken(token);
                // attach account to context on successful jwt validation
                var nesto = await dataContext.Accounts.FindAsync(Convert.ToInt32(accountId));
                context.Items["Account"] = nesto;
            }
            catch 
            {
                // do nothing if jwt validation fails
                // account is not attached to context so request won't have access to secure routes
            }
        }
    }
}