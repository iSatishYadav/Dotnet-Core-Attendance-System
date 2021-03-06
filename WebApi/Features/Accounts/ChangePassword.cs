using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using WebApi.Entities;

namespace WebApi.Features.Accounts
{
    /// <summary>
    /// Change account password
    /// </summary>
    public class ChangePassword
    {
        public class Command : IRequest<IdentityResult>
        {
            public Command(ChangePasswordViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public ChangePasswordViewModel ViewModel { get; }
        }

        public class CommandHandler : IRequestHandler<Command, IdentityResult>
        {
            private readonly UserManager<User> _manager;
            private readonly IHttpContextAccessor _httpContext;

            public CommandHandler(UserManager<User> manager, IHttpContextAccessor httpContext)
            {
                _manager = manager;
                _httpContext = httpContext;
            }

            public async Task<IdentityResult> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    // Get the username in sub type claim
                    var username = _httpContext.HttpContext.User.Claims
                        .First(m => m.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                        .Value;

                    // Get account details
                    var user = await _manager.FindByNameAsync(username);

                    // Change account password then return result
                    return await _manager.ChangePasswordAsync(
                        user, 
                        request.ViewModel.OldPassword, 
                        request.ViewModel.NewPassword);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}