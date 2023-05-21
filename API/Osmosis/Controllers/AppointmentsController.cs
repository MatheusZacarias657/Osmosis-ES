using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osmosis.Controllers.Base;
using Osmosis.DAO;
using Osmosis.DAO.Base;
using Osmosis.Models;
using Osmosis.Services;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Osmosis.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AppointmentsController : CRUDController<Appointment>
    {
        private StatusDAO _statusDAO;
        private DailyAppointmentDAO _dailyAppointmentDAO;
        private AppointmentDAO _appointmentDAO;
        private DoctorsDAO _doctorsDAO;

        public AppointmentsController(DataContext datacontext, IBaseDAO<Appointment> genericDAO) : base(genericDAO)
        {
            _statusDAO = new StatusDAO(datacontext);
            _dailyAppointmentDAO = new DailyAppointmentDAO(datacontext);
            _appointmentDAO = new AppointmentDAO(datacontext);
            _doctorsDAO = new DoctorsDAO(datacontext);
            AppointmentService.InicializeDAO(datacontext);
        }

        [HttpGet]
        [Authorize("AllUsers")]
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

        [HttpPost]
        [Authorize("AllUsers")]
        [ActionName("")]
        public override ActionResult Create(Appointment appointment)
        {
            try
            {
                _doctorsDAO.FindById(appointment.id_doctor);
                
                if (!AppointmentService.ValidPeriod(appointment))
                    return BadRequest("The selected period is invalid");

                _appointmentDAO.Add(appointment);

                return Created("", null);
            } 
            catch (Exception error) 
            {
                throw error;
            }
        }

        [HttpPut]
        [Authorize("AllUsers")]
        [ActionName("")]
        public override ActionResult Update(Appointment appointment)
        {
            try
            {
                if (appointment.id_doctor != default(int))
                    _doctorsDAO.FindById(appointment.id_doctor);

                if (!AppointmentService.ValidPeriod(appointment))
                    return BadRequest("The selected period is invalid");

                Appointment oldAppointment = _appointmentDAO.FindById(appointment.id);
                Status status = _statusDAO.FindById(appointment.id_status);

                if (status.createDocument)
                {
                    oldAppointment.id_status = status.id;
                    _appointmentDAO.Update(oldAppointment);
                    GenericServices.CopyProperties(oldAppointment, appointment);
                    oldAppointment.id_status = 1;
                    oldAppointment.id = default(int);
                    _appointmentDAO.Add(oldAppointment);
                } 
                else
                {
                    GenericServices.CopyProperties(oldAppointment, appointment);
                    _appointmentDAO.Update(oldAppointment);
                }

                return Ok();
            }
            catch (Exception error)
            {
                throw error;
            }
        }

        [HttpGet]
        [Authorize("AllUsers")]
        public ActionResult GetAvailable(int id_doctor, DateTime dateStart, [Optional] DateTime? dateEnd)
        {
            try
            {
                if (dateEnd.HasValue && dateEnd <= dateStart) 
                    return BadRequest("The period is invalid");

                if (!dateEnd.HasValue)
                    dateEnd = dateStart;

                _doctorsDAO.FindById(id_doctor);
                List<Appointment> availableAppointments = AppointmentService.FindAvailble(id_doctor, dateStart, dateEnd.Value);

                return Ok(availableAppointments);
            } 
            catch(Exception error) 
            {
                throw error;
            }
        }
    }
}
