using Osmosis.DAO.Base;
using Osmosis.Models;

namespace Osmosis.DAO
{
    public class UserRolesDAO
    {
        private DataContext _datacontext;

        public UserRolesDAO(DataContext datacontext)
        {
            _datacontext = datacontext;
        }

        public UserRoles GetRole(string roleName)
        {
            try
            {
                int idRole = -1;
                try
                {
                    idRole = Convert.ToInt32(roleName);
                }
                catch(Exception ex){ }
                
                UserRoles role = _datacontext.UserRoles.Where(x => x.name.ToLower().Equals(roleName.ToLower()) || x.id == idRole).FirstOrDefault();

                if (role == null)
                    throw new Exception("Role doesn't exist");

                return role;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
