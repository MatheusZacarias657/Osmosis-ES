using Osmosis.DAO.Base;
using Osmosis.Models;

namespace Osmosis.DAO
{
    public class DoctorsDAO : BaseDAO<Doctors>
    {
        public DoctorsDAO(DataContext datacontext) : base(datacontext) { }
    }
}
