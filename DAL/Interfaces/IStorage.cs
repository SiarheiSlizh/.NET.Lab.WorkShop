using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IStorage
    {
        void Add(PortfolioItemDAL item);
        void Update(PortfolioItemDAL item);
        void Delete(int id);
        IEnumerable<PortfolioItemDAL> GetAll();
        IEnumerable<PortfolioItemDAL> GetByPredicate(Func<PortfolioItemDAL, bool> predicate);
        PortfolioItemDAL GetById(int id);
    }
}
