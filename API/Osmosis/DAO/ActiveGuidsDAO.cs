using Osmosis.DAO.Base;
using Osmosis.Models;
using Osmosis.Services;

namespace Osmosis.DAO
{
    public class ActiveGuidsDAO : BaseDAO<ActiveGuids>
    {
        public ActiveGuidsDAO(DataContext datacontext) : base(datacontext) { }

        public ActiveGuids FindSessionByGuid (string guid)
        {
            try
            {
                DateTime now = GenericServices.GetCurrentDateTime();
                ActiveGuids session = _datacontext.ActiveGuids.Where(x => x.guid.Equals(guid) && now  < x.expirationDate.Value).FirstOrDefault();

                if (session == null)
                    throw new Exception("user not logged");

                return session;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveSessionsByUser(int id)
        {
            try
            {
                List<ActiveGuids> sessions = _datacontext.ActiveGuids.Where(x => x.id_user == id).ToList();
                DateTime now = GenericServices.GetCurrentDateTime();
                sessions.ForEach(x => x.expirationDate = now);
                _datacontext.ActiveGuids.UpdateRange(sessions);
                _datacontext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
