using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortfolioManagerClient.Models
{
    public class StockViewModel
    {
        public string Ticker { get; set; }
        public decimal Price { get; set; }
    }
}