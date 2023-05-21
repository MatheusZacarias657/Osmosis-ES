using Osmosis.DAO.Base;
using Osmosis.Models;

namespace Osmosis.DAO
{
    public class DailyAppointmentDAO: BaseDAO<DailyAppointments> 
    {
        private DataContext _datacontext;
        public DailyAppointmentDAO(DataContext datacontext) : base(datacontext) 
        {
            _datacontext = datacontext;
        }

        public List<DailyAppointments> GetDailyAppointmentByDoctorId(int id)
        {
            try
            {
                List<DailyAppointments>? dailyAppointment = _datacontext.DailyAppointments.Where(dailyAppointment => dailyAppointment.id_doctor == id).ToList();
                if (dailyAppointment == null) 
                    throw new Exception("Nenhum médico encontrado.");
                return dailyAppointment;
             }
            catch (Exception error)
            {
                throw error;
            }
        }
    }
}
