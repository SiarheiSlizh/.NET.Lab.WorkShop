using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioManager.Service.Models;

namespace PortfolioManager.Service.Interfaces
{
    public interface IService
    {
        void Add(PortfolioBllModel item);
        void Update(PortfolioBllModel item);
        void Delete(int id);
        IEnumerable<PortfolioBllModel> GetAll(int userId);
        PortfolioBllModel GetById(int id);
    }
}
