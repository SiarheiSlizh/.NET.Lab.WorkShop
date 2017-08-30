using DAL.DTO;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCUI
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlLocalStorage loc = new XmlLocalStorage(@"E:\.NET Lab\.NET.Lab.WorkShop-master\DAL\storage.xml");
            var list = loc.GetAll(1);
            Console.WriteLine(list.Count());

            foreach (var item in list)
                Console.WriteLine(item.ItemId + " " + item.UserId + " " + item.Symbol + " " + item.SharesNumber);

            PortfolioItemDAL el = loc.GetById(2);

            Console.WriteLine(el.ItemId + " " + el.UserId + " " + el.Symbol + " " + el.SharesNumber);

            //loc.Delete(3);

            PortfolioItemDAL newEl = new PortfolioItemDAL() { ItemId = 5, UserId = 2, Symbol = "epam", SharesNumber = 100 };
            loc.Add(newEl);

            //loc.Update(newEl);

        }
    }
}
