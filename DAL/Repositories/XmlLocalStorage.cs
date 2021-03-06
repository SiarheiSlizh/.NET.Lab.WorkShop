﻿using DAL.DTO;
using DAL.Interfaces;
using DAL.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace DAL.Repositories
{
    public class XmlLocalStorage : IStorage
    {
        private ReaderWriterLockSlim lockRW = new ReaderWriterLockSlim();
        private string path;
        private int counter;

        public XmlLocalStorage(string path)
        {
            this.path = path;
            if (this.GetAll().Count() == 0)
                counter = 1;
            else
                this.counter = GetAll().Select(m => m.ItemId).Max();
        }

        public void Add(PortfolioItemDAL item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            
            XmlDocument xDoc = new XmlDocument();
            lockRW.EnterWriteLock();
            try
            {
                item.ItemId = ++counter;
                xDoc.Load(path);
                XmlElement xRoot = xDoc.DocumentElement;
                int count = xRoot.ChildNodes.Count;

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
                XmlElement xRemoteId = xDoc.CreateElement("RemoteId");
                xItem.AppendChild(xRemoteId);
                XmlElement xStatus = xDoc.CreateElement("Status");
                xStatus.InnerText = SyncronizationStatus.New.ToString();
                xItem.AppendChild(xStatus);
            
                xRoot.AppendChild(xItem);
                xDoc.Save(path);
            }
            finally
            {
                lockRW.ExitWriteLock();
            }
        }

        public void Delete(int id)
        {
            XmlDocument xDoc = new XmlDocument();
            lockRW.EnterWriteLock();
            try
            {
                xDoc.Load(path);
                XmlElement xRoot = xDoc.DocumentElement;            
                xRoot.RemoveChild(xRoot.SelectSingleNode("Item[ItemId='" + id + "']"));
                xDoc.Save(path);
            }
            finally
            {
                lockRW.ExitWriteLock();
            }
        }

        public IEnumerable<PortfolioItemDAL> GetAll()
        {
            XmlDocument xDoc = new XmlDocument();
            lockRW.EnterReadLock();
            try {
                xDoc.Load(path);
                XmlElement xRoot = xDoc.DocumentElement;
                XmlNodeList nodes = xRoot.ChildNodes;

                List<PortfolioItemDAL> list = new List<PortfolioItemDAL>();

                foreach (XmlNode node in nodes)
                {
                    int remoteId;
                    int.TryParse(node.ChildNodes[4]?.InnerText, out remoteId);
                    list.Add(new PortfolioItemDAL()
                    {
                        ItemId = int.Parse(node.ChildNodes[0].InnerText),
                        UserId = int.Parse(node.ChildNodes[1].InnerText),
                        Symbol = node.ChildNodes[2].InnerText,
                        SharesNumber = int.Parse(node.ChildNodes[3].InnerText),
                        RemoteId = remoteId,
                        Status = node.ChildNodes[5].InnerText.MapToSyncStatus()
                    });
                }

                return list;
            }
            finally
            {
                lockRW.ExitReadLock();
            }
        }

        public IEnumerable<PortfolioItemDAL> GetByPredicate(Func<PortfolioItemDAL, bool> predicate)
        {
            IEnumerable<PortfolioItemDAL> list = new List<PortfolioItemDAL>();
            list = GetAll();
            return list.Where(predicate);
        }

        public PortfolioItemDAL GetById(int id)
        {
            XmlDocument xDoc = new XmlDocument();
            lockRW.EnterReadLock();
            try
            {
                xDoc.Load(path);
                XmlElement xRoot = xDoc.DocumentElement;
            
                XmlNode node = xRoot.SelectSingleNode("Item[ItemId='" + id + "']");

                int remoteId;
                int.TryParse(node.ChildNodes[4]?.InnerText, out remoteId);
                return new PortfolioItemDAL()
                {
                    ItemId = int.Parse(node.ChildNodes[0].InnerText),
                    UserId = int.Parse(node.ChildNodes[1].InnerText),
                    Symbol = node.ChildNodes[2].InnerText,
                    SharesNumber = int.Parse(node.ChildNodes[3].InnerText),
                    RemoteId = remoteId,
                    Status = node.ChildNodes[5].InnerText.MapToSyncStatus()
                };
            }
            finally
            {
                lockRW.ExitReadLock();
            }
        }

        public void Update(PortfolioItemDAL item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            XmlDocument xDoc = new XmlDocument();
            lockRW.EnterWriteLock();
            try {
                xDoc.Load(path);
                XmlElement xRoot = xDoc.DocumentElement;
                XmlNode node = xRoot.SelectSingleNode("Item[ItemId='" + item.ItemId + "']");

                node.ChildNodes[1].InnerText = item.UserId.ToString();
                node.ChildNodes[2].InnerText = item.Symbol;
                node.ChildNodes[3].InnerText = item.SharesNumber.ToString();
                node.ChildNodes[4].InnerText = item.RemoteId.ToString();
                node.ChildNodes[5].InnerText = item.Status.ToString();

                xDoc.Save(path);
            }
            finally
            {
                lockRW.ExitWriteLock();
            }
        }
    }
}
