using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osmosis.Controllers.Base;
using Osmosis.DAO;
using Osmosis.DAO.Base;
using Osmosis.Models;
using Osmosis.Services;

namespace Osmosis.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OAuthController : ControllerBase
    {
        private UserDAO _userDAO;
        private UserRolesDAO _userRolesDAO;
        private ActiveGuidsDAO _activeGuidsDAO;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OAuthController (DataContext datacontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userDAO = new UserDAO(datacontext);
            _userRolesDAO = new UserRolesDAO(datacontext);
            _activeGuidsDAO = new ActiveGuidsDAO(datacontext);
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login([FromBody] UserLogin login)
        {
            try
            {
                Users user = _userDAO.FindUserByLoginOrEmail(login.username);
                login.password = OAuthService.GenerateSaltedHash(login.password, user.login);

                if (!login.password.Equals(user.password))
                    return NotFound();

                ActiveGuids session = new ActiveGuids()
                {
                    id_user = user.id,
                    creationDate = GenericServices.GetCurrentDateTime(),
                    guid = Guid.NewGuid().ToString()
                };

                _activeGuidsDAO.Add(session);
                string roleName = _userRolesDAO.GetRole(user.id_role.ToString()).name;
                OAuthService.AuthorizeUser(roleName, _httpContextAccessor);

                return Ok(new { guid = session.guid, login = user.login });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Authorize("AllUsers")]
        public ActionResult Logout([FromBody] UserLogin login)
        {
            try
            {
                Users user = _userDAO.FindUserByLoginOrEmail(login.username);
                login.password = OAuthService.GenerateSaltedHash(login.password, user.login);

                if (!login.password.Equals(user.password))
                    return NotFound();

                _activeGuidsDAO.RemoveSessionsByUser(user.id);

                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgetPassword(string login, string email)
        {
            try
            {
                Users user = _userDAO.FindUserByLoginOrEmail(login);

                if (!email.Equals(user.email))
                    return Forbid();

                List<Users> admins = _userDAO.FindByCustomFilter(new Dictionary<string, string> { { "id_role", "1" } });
                OAuthService.SendEmail(_configuration, user, admins);
                _activeGuidsDAO.RemoveSessionsByUser(user.id);

                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
