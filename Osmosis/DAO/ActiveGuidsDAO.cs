using Osmosis.DAO.Base;
using Osmosis.Models;
using Osmosis.Services;

namespace Osmosis.DAO
{
    public class ActiveGuidsDAO : BaseDAO<ActiveGuids>
    {
        private DataContext _datacontext;

        public ActiveGuidsDAO(DataContext datacontext) : base(datacontext)
        {
            _datacontext = datacontext;
        }

        public ActiveGuids FindSessionByGuid (string guid)
        {
            try
            {
                ActiveGuids session = _datacontext.ActiveGuids.Where(x => x.guid.Equals(guid) && GenericServices.GetCurrentDateTime() < x.expirationDate).FirstOrDefault();

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
                sessions.ForEach(x => x.expirationDate = GenericServices.GetCurrentDateTime());
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
