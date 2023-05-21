
namespace Osmosis.DAO.Base
{
    public class BaseDAO<T> : IBaseDAO<T> where T : class
    {
        protected readonly DataContext _datacontext;

        public BaseDAO(DataContext dataContext)
        {
            _datacontext = dataContext;
        }

        public void Add(T entity)
        {
            try
            {
                _datacontext.Set<T>().Add(entity);
                _datacontext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(T entity)
        {
            try
            {
                _datacontext.Set<T>().Update(entity);
                _datacontext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T FindById(int id)
        {
            try
            {
                T entity =  _datacontext.Set<T>().Find(id);

                if (entity == null)
                    throw new Exception("Register not exist");

                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T> FindByCustomFilter(Dictionary<string, string> filters)
        {
            try
            {
                List<T> filteredList = filters.Aggregate(_datacontext.Set<T>().AsEnumerable(), (current, filter) => current.Where(a => a.GetType().GetProperty(filter.Key).GetValue(a).ToString().ToLower() == filter.Value.ToLower())).ToList();

                return filteredList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T> GetAllRegisters()
        {
            try
            {
                return _datacontext.Set<T>().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
