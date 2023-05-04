using Microsoft.AspNetCore.Authorization;
using Osmosis.DAO;
using Osmosis.DAO.Base;
using Osmosis.Models;
using Osmosis.Services;
using System.Text.Json;

namespace Osmosis.Authorization
{
    public class APIAuthorization : IAuthorizationRequirement { }

    public class APIAuthorizationHandler : AuthorizationHandler<APIAuthorization>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserDAO _userDAO;
        private ActiveGuidsDAO _activeGuidsDAO;
        private UserRolesDAO _userRolesDAO;

        public APIAuthorizationHandler(IHttpContextAccessor httpContextAccessor, DataContext dataContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _userDAO = new UserDAO(dataContext);
            _activeGuidsDAO = new ActiveGuidsDAO(dataContext);
            _userRolesDAO = new UserRolesDAO(dataContext);
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, APIAuthorization requirement)
        {
            try
            {
                string userGuid = _httpContextAccessor.HttpContext.Request.Headers["token"];

                if (string.IsNullOrEmpty(userGuid))
                    return Task.CompletedTask;

                string roleName = string.Empty;
                try
                {
                    ActiveGuids session = _activeGuidsDAO.FindSessionByGuid(userGuid);
                    int id_role = _userDAO.FindById(session.id_user).id_role;
                    roleName = _userRolesDAO.GetRole(id_role.ToString()).name;
                }
                catch (Exception ex)
                {
                    return Task.CompletedTask;
                }
                

                IAuthorizationRequirement contextRequirement = context.Requirements.ElementAt(1);
                object allowedRolesProperties = contextRequirement.GetType().GetProperty("AllowedRoles").GetValue(contextRequirement);
                List<string> allowedRoles = JsonSerializer.Deserialize<List<string>>(JsonSerializer.Serialize(allowedRolesProperties));

                if (allowedRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase))
                {
                    UserService.AuthorizeUser(roleName, _httpContextAccessor);
                    context.Succeed(requirement);
                }

                return Task.CompletedTask;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
