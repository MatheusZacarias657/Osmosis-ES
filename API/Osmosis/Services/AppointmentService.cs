using Azure.Core;
using Osmosis.DAO;
using Osmosis.DAO.Base;
using Osmosis.Models;
using System.Runtime.InteropServices;

namespace Osmosis.Services
{
    public class AppointmentService
    {
        private static DailyAppointmentDAO _dailyAppointmentDAO;
        private static AppointmentDAO _appointmentDAO;

        public static void InicializeDAO(DataContext dataContext)
        {
            _dailyAppointmentDAO = new DailyAppointmentDAO(dataContext);
            _appointmentDAO = new AppointmentDAO(dataContext);
        }

        public static bool ValidPeriod(Appointment appointment)
        {
            if (appointment.appointmentTime.HasValue && appointment.appointmentTime.Value < GenericServices.GetCurrentDateTime())
                return false;

            List<DailyAppointments> dailyAppointments = _dailyAppointmentDAO.GetDailyAppointmentByDoctorId(appointment.id_doctor);
            if (appointment.appointmentTime.HasValue && !dailyAppointments.Exists(dailyAppointment => appointment.appointmentTime.Value.TimeOfDay == dailyAppointment.startTime))
                return false;

            if (appointment.appointmentTime.HasValue)
            {
                bool isValidAppointment = _appointmentDAO.ValidAppointment(appointment);
                if (!isValidAppointment)
                    return false;
            }

            return true;
        }

        public static List<Appointment> FindAvailble(int id_doctor, DateTime dateStart, DateTime dateEnd)
        {

            List<Appointment> activeAppointments = _appointmentDAO.FindAppointmentsByDoctorId(id_doctor, dateStart, dateEnd).OrderBy(activeAppointment => activeAppointment.appointmentTime).ToList();

            List<DailyAppointments> dailyAppointments = _dailyAppointmentDAO.GetDailyAppointmentByDoctorId(id_doctor);
            List<Appointment> availableAppointments = new List<Appointment>();

            if (activeAppointments.Count == 0)
            {
                for (DateTime i = dateStart; i <= dateEnd; i = i.AddDays(1))
                {
                    foreach (DailyAppointments dailyAppointment in dailyAppointments)
                    {
                        Appointment appointment = new Appointment
                        {
                            id_doctor = id_doctor,
                            appointmentTime = new DateTime(i.Year, i.Month, i.Day, dailyAppointment.startTime.Hours, dailyAppointment.startTime.Minutes, dailyAppointment.startTime.Seconds)
                        };

                        availableAppointments.Add(appointment);
                    }
                }
            }
            else
            {
                for (DateTime i = dateStart; i <= dateEnd; i = i.AddDays(1))
                {
                    List<Appointment> dayAppointments = activeAppointments.Where(activeAppointment => activeAppointment.appointmentTime.Value.Date == i.Date).ToList();
                    foreach (DailyAppointments dailyAppointment in dailyAppointments)
                    {
                        if (!dayAppointments.Exists(dayAppointment => dayAppointment.appointmentTime.Value.TimeOfDay == dailyAppointment.startTime))
                        {
                            Appointment appointment = new Appointment
                            {
                                id_doctor = id_doctor,
                                appointmentTime = new DateTime(i.Year, i.Month, i.Day, dailyAppointment.startTime.Hours, dailyAppointment.startTime.Minutes, dailyAppointment.startTime.Seconds)
                            };

                            availableAppointments.Add(appointment);
                        }
                    }
                }
            }

            return availableAppointments; 
        }
    }
}
