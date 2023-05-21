using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osmosis.Controllers.Base;
using Osmosis.DAO;
using Osmosis.DAO.Base;
using Osmosis.Models;
using Osmosis.Services;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Osmosis.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : CRUDController<Users>
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
        [Authorize("RequireAdmin")]
        [ActionName("")]
        public override ActionResult Create([FromBody] Users user)
        {
            try
            {
                if (!Regex.IsMatch(user.email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$") ||
                    !Regex.Match(user.email, @"^([^@]+)").Value.Equals(user.login) ||
                    !_userDAO.VerifyLoginIsUnique(user.login))
                    return BadRequest();

                user.password = OAuthService.GenerateSaltedHash(user.password, user.login);
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

                if (!Regex.IsMatch(olduser.email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$") ||
                    !Regex.Match(olduser.email, @"^([^@]+)").Value.Equals(olduser.login) ||
                    _userDAO.FindByCustomFilter(new Dictionary<string, string> { { "login", olduser.login } }).Count > 1)
                    return BadRequest();

                olduser.password = OAuthService.GenerateSaltedHash(olduser.password, olduser.login);
                olduser.id = 2345;
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
        public ActionResult Read([FromQuery] Dictionary<string, string>? filters)
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
