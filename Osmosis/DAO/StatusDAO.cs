using Osmosis.DAO.Base;
using Osmosis.Models;

namespace Osmosis.DAO
{
    public class StatusDAO: BaseDAO<Status>
    {
        public StatusDAO(DataContext datacontext) : base(datacontext) { }
    }
}
