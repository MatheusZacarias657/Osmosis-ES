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
    public class DoctorController : CRUDController<Doctors>
    {
        private DoctorsDAO _doctorsDAO;
        private AppointmentDAO _appointmentDAO;

        public DoctorController(DataContext datacontext, IBaseDAO<Doctors> genericDAO) : base(genericDAO)
        {
            _doctorsDAO = new DoctorsDAO(datacontext);
            _appointmentDAO = new AppointmentDAO(datacontext);
        }

        [HttpPost]
        [Authorize ("RequireAdmin")]
        [ActionName ("")]
        public override ActionResult Create(Doctors doctor)
        {
            try
            {
                if (doctor.entryTime > doctor.departureTime)
                    return BadRequest("Invalid office hours");

                _doctorsDAO.Add(doctor);

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
        public override ActionResult Update(Doctors doctor)
        {
            try
            {
                Doctors oldDoctor = _doctorsDAO.FindById(doctor.id);
                GenericServices.CopyProperties(oldDoctor, doctor);

                if (oldDoctor.entryTime > oldDoctor.departureTime)
                    return BadRequest("Invalid office hours");

                _doctorsDAO.Update(oldDoctor);

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
                _appointmentDAO.CloseAppointmentsByDoctorId(id);
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
        public ActionResult Get([FromQuery] Dictionary<string, string>? filters)
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
