using Microsoft.AspNetCore.Mvc;
using Osmosis.DAO.Base;

namespace Osmosis.Controllers.Base
{
    public abstract class GenericController <T> : ControllerBase where T : class
    {
        protected IBaseDAO<T> _genericDAO;

        public GenericController(IBaseDAO<T> genericDAO)
        {
            _genericDAO = genericDAO;
        }

        public abstract ActionResult Create(T entity);

        protected ActionResult Read(Dictionary<string, string>? filters)
        {
            try
            {
                if (filters.Count == 0)
                    return Ok(_genericDAO.GetAllRegisters());

                if (!filters.ContainsKey("active"))
                    filters.Add("active", "true");

                return Ok(_genericDAO.FindByCustomFilter(filters));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public abstract ActionResult Update(T entity);

        protected ActionResult Delete(int id)
        {
            try
            {
                T entity = _genericDAO.FindById(id);
                entity.GetType().GetProperty("active").SetValue(entity, false);
                _genericDAO.Update(entity);

                return NoContent();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
