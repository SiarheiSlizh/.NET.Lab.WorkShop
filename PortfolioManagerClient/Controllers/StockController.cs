using Newtonsoft.Json.Linq;
using PortfolioManagerClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PortfolioManagerClient.Controllers
{
    public class StockController : ApiController
    {
        //strind variants - AAPL,GOOG,GOOGL,YHOO,TSLA,INTC,AMZN,BIDU,ORCL,MSFT,ORCL,ATVI,NVDA,GME,LNKD,NFLX
        [Route("api/Stock/{name}")]
        public StockViewModel Get(string name)
        {
            string json;

            using (var web = new WebClient())
            {
                var url = string.Format("http://finance.google.com/finance/info?q=NASDAQ:{0}", name);
                json = web.DownloadString(url);
            }

            //Google adds a comment before the json for some unknown reason, so we need to remove it
            json = json.Replace("//", "");

            var v = JArray.Parse(json);

            var ticker = (string)v[0].SelectToken("t");
            var price = (decimal)v[0].SelectToken("l");


            return new StockViewModel()
            {
                Ticker = ticker,
                Price = price
            };
        }
    }
}
