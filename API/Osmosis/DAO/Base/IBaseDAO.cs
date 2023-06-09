﻿namespace Osmosis.DAO.Base
{
    public interface IBaseDAO<T> where T : class
    {
        void Update(T entity);
        T FindById(int id);
        List<T> FindByCustomFilter(Dictionary<string, string> filters);
        List<T> GetAllRegisters();
    }
}
