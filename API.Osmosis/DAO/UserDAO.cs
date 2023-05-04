using Osmosis.DAO.Base;
using Osmosis.Models;
using System.Data;

namespace Osmosis.DAO
{
    public class UserDAO : BaseDAO<Users>
    {
        private DataContext _datacontext;

        public UserDAO(DataContext datacontext) : base(datacontext)
        {
            _datacontext = datacontext;
        }

        public bool VerifyLoginIsUnique(string login)
        {
            try
            {
                int exists = (from user in _datacontext.Users
                              where user.login.Equals(login) && user.active
                              select user).ToList().Count;

                return (exists != 0 ) ? false : true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Users FindUserByLoginOrEmail(string credential)
        {
            try
            {
                Users? userResponse = (from user in _datacontext.Users
                                       where (user.login.Equals(credential) || user.email.Equals(credential)) && user.active == true
                                       select user).ToList().FirstOrDefault();

                if (userResponse == null)
                    throw new Exception("User not exist");

                return userResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
