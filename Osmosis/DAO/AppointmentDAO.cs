using Osmosis.DAO.Base;
using Osmosis.Models;
using System.Net.NetworkInformation;

namespace Osmosis.DAO
{
    public class AppointmentDAO: BaseDAO<Appointment> 
    {
        private DataContext _datacontext;
        public AppointmentDAO(DataContext datacontext) : base(datacontext) 
        {
            _datacontext = datacontext;
        }

        public List<Appointment> FindAppointmentsByDoctorId(int id, DateTime date)
        {
            try
            {
                List<Appointment>? appointments = _datacontext.Appointments.Where(appointment => 
                                                                            appointment.id_doctor == id 
                                                                            && appointment.appointmentTime.Value.Date == date.Date
                                                                            && appointment.id_status == 1).ToList();
                return appointments;
            }
            catch(Exception error)
            {
                throw error;
            }
        }

        public List<Appointment> FindAppointmentsByDoctorId(int id, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                List<Appointment>? appointments = _datacontext.Appointments.Where(appointment =>
                                                                            appointment.id_doctor == id
                                                                            && appointment.appointmentTime.Value.Date >= dateStart.Date
                                                                            && appointment.appointmentTime.Value.Date <= dateEnd.Date
                                                                            && appointment.id_status == 1).ToList();
                return appointments;
            }
            catch (Exception error)
            {
                throw error;
            }
        }

        public bool ValidAppointment(Appointment appointmentToValid)
        {
            try
            {
                int validAppointment = _datacontext.Appointments.Where(appointment =>
                                                                            appointment.id_doctor == appointmentToValid.id_doctor
                                                                            && appointment.appointmentTime == appointmentToValid.appointmentTime
                                                                            && appointment.id_status == 1).ToList().Count;

                return validAppointment != 0 ? false : true;
            }
            catch (Exception error)
            {
                throw error;
            }
        }
    }
}
