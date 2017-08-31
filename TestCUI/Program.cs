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
            XmlLocalStorage loc = new XmlLocalStorage(@"E:\.NET Lab\Topics\fff\.NET.Lab.WorkShop-master\DAL\storage.xml");
            //var list = loc.GetByPredicate(m => m.Status != SyncronizationStatus.Deleted);
            //Console.WriteLine(list.Count());

            //foreach (var item in list)
            //    Console.WriteLine(item.ItemId + " " + item.UserId + " " + item.Symbol + " " + item.SharesNumber + " " + item.RemoteId + " " +item.Status);

            //PortfolioItemDAL el = loc.GetById(1);

            //Console.WriteLine(el.ItemId + " " + el.UserId + " " + el.Symbol + " " + el.SharesNumber + " " + el.RemoteId + " " + el.Status);

            //loc.Delete(2);

            PortfolioItemDAL newEl = new PortfolioItemDAL() { UserId = 2, Symbol = "epam", SharesNumber = 120, Status = SyncronizationStatus.Syncronized };
            loc.Add(newEl);

            //loc.Update(newEl);
            //var el = loc.GetById(5);
            //Console.WriteLine(el.ItemId + " " + el.UserId + " " + el.Symbol + " " + el.SharesNumber);
        }
    }
}
