using DAL.DTO;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DAL.Repositories
{
    public class XmlLocalStorage : IStorage
    {
        private string path;

        public XmlLocalStorage(string path)
        {
            this.path = path;
        }

        public void Add(PortfolioItemDAL item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            XmlElement xRoot = xDoc.DocumentElement;

            XmlElement xItem = xDoc.CreateElement("Item");

            XmlElement xItemId = xDoc.CreateElement("ItemId");
            xItemId.InnerText = item.ItemId.ToString();
            xItem.AppendChild(xItemId);
            XmlElement xUserId = xDoc.CreateElement("UserId");
            xUserId.InnerText = item.UserId.ToString();
            xItem.AppendChild(xUserId);
            XmlElement xSymbol = xDoc.CreateElement("Symbol");
            xSymbol.InnerText = item.Symbol;
            xItem.AppendChild(xSymbol);
            XmlElement xSharesNumber = xDoc.CreateElement("SharesNumber");
            xSharesNumber.InnerText = item.SharesNumber.ToString();
            xItem.AppendChild(xSharesNumber);

            xRoot.AppendChild(xItem);
            xDoc.Save(path);
        }

        public void Delete(int id)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            XmlElement xRoot = xDoc.DocumentElement;

            xRoot.RemoveChild(xRoot.SelectSingleNode("Item[ItemId='" + id + "']"));
            xDoc.Save(path);
        }

        public IEnumerable<PortfolioItemDAL> GetAll(int userId)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNodeList nodes = xRoot.SelectNodes("Item[UserId='" + userId + "']");

            List<PortfolioItemDAL> list = new List<PortfolioItemDAL>();

            foreach (XmlNode node in nodes)
            {
                list.Add(new PortfolioItemDAL()
                {
                    ItemId = int.Parse(node.ChildNodes[0].InnerText),
                    UserId = int.Parse(node.ChildNodes[1].InnerText),
                    Symbol = node.ChildNodes[2].InnerText,
                    SharesNumber = int.Parse(node.ChildNodes[3].InnerText)
                });
            }

            return list;
        }

        public PortfolioItemDAL GetById(int id)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNode node = xRoot.SelectSingleNode("Item[ItemId='" + id + "']");

            return new PortfolioItemDAL()
            {
                ItemId = int.Parse(node.ChildNodes[0].InnerText),
                UserId = int.Parse(node.ChildNodes[1].InnerText),
                Symbol = node.ChildNodes[2].InnerText,
                SharesNumber = int.Parse(node.ChildNodes[3].InnerText)
            };
        }

        public void Update(PortfolioItemDAL item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNode node = xRoot.SelectSingleNode("Item[ItemId='" + item.ItemId + "']");

            node.ChildNodes[1].InnerText = item.UserId.ToString();
            node.ChildNodes[2].InnerText = item.Symbol;
            node.ChildNodes[3].InnerText = item.SharesNumber.ToString();

            xDoc.Save(path);
        }
    }
}
