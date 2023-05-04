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
    public class UserController : GenericController<Users>
    {
        private UserDAO _userDAO;
        private UserRolesDAO _userRolesDAO;
        private ActiveGuidsDAO _activeGuidsDAO;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController (DataContext datacontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IBaseDAO<Users> genericDAO) : base(genericDAO)
        {
            _userDAO = new UserDAO(datacontext);
            _userRolesDAO = new UserRolesDAO(datacontext);
            _activeGuidsDAO = new ActiveGuidsDAO(datacontext);
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login (string username, string password)
        {
            try
            {
                Users user = _userDAO.FindUserByLoginOrEmail(username);
                password = UserService.GenerateSaltedHash(password, user.login);

                if (!password.Equals(user.password))
                    return Forbid();

                ActiveGuids session = new ActiveGuids()
                {
                    id_user = user.id,
                    creationDate = DateTime.Now,
                    guid = Guid.NewGuid().ToString()
                };

                _activeGuidsDAO.Add(session);
                string roleName = _userRolesDAO.GetRole(user.id_role.ToString()).name;
                UserService.AuthorizeUser(roleName, _httpContextAccessor);

                return Ok(new { guid = session.guid, login = user.login });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Authorize("RequireAdmin")]
        [ActionName("")]
        public override ActionResult Create([FromBody] Users user)
        {
            try
            {
                string login = user.email.Split('@')[0];

                if (string.IsNullOrEmpty(login) || !login.Equals(user.login))
                    return BadRequest();

                if (!_userDAO.VerifyLoginIsUnique(user.login))
                    return BadRequest();

                user.password = UserService.GenerateSaltedHash(user.password, user.login);
                _userDAO.Add(user);

                return Created("", null);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut]
        [Authorize("RequireAdmin")]
        [ActionName("")]
        public override ActionResult Update([FromBody] Users user)
        {
            try
            {
                Users olduser = _userDAO.FindById(user.id);
                GenericServices.CopyProperties(olduser, user);
                olduser.password = UserService.GenerateSaltedHash(olduser.password, olduser.login);
                _userDAO.Update(olduser);

                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpDelete]
        [Authorize("RequireAdmin")]
        [ActionName("")]
        public ActionResult Delete(int id)
        {
            try
            {
                return base.Delete(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Authorize("RequireAdmin")]
        [ActionName("")]
        public ActionResult Delete([FromQuery] Dictionary<string, string>? filters)
        {
            try
            {
                return base.Read(filters);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
