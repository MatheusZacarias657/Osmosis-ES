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
    public class HealthController : ControllerBase
    {

        [HttpGet]
        [AllowAnonymous]
        public ActionResult IsAlive()
        {
            try
            {
                return Ok(new { alive = true });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
