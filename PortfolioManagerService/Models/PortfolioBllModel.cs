using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO;

namespace PortfolioManager.Service.Models
{
    public class PortfolioBllModel
    {
        public int ItemId { get; set; }

        public int UserId { get; set; }

        public string Symbol { get; set; }

        public int SharesNumber { get; set; }

        public int RemoteId { get; set; }

        public SyncronizationStatus Status { get; set; }
    }
}
