using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortfolioManager.Service.Interfaces;
using PortfolioManager.Service.Models;
using PortfolioManager.Service.Infrastructure;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Ninject;
using DAL.Interfaces;
using DAL.Repositories;
using System.Threading;
using System.Threading.Tasks;
using PortfolioManagerService.Models;

namespace PortfolioManager.Service
{
    public class PortfolioService : IService
    {
        /// <summary>
        /// The url for getting all portfolio items.
        /// </summary>
        private const string GetAllUrl = "PortfolioItems?userId={0}";

        /// <summary>
        /// The url for updating a portfolio item.
        /// </summary>
        private const string UpdateUrl = "PortfolioItems";

        /// <summary>
        /// The url for a portfolio item's creation.
        /// </summary>
        private const string CreateUrl = "PortfolioItems";

        /// <summary>
        /// The url for a portfolio item's deletion.
        /// </summary>
        private const string DeleteUrl = "PortfolioItems/{0}";

        /// <summary>
        /// The service URL.
        /// </summary>
        private readonly string _serviceApiUrl = "http://portfolio-manager.azurewebsites.net/api/";

        /// <summary>
        /// The timeout of syncronization in miliseconds
        /// </summary>
        private readonly int cloudUpdateTimeOut = 10000;

        private readonly HttpClient _httpClient;
        private readonly IStorage storage;
        public PortfolioService (IStorage storage)
        {
            this.storage = storage;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Task.Run((Action)UploadDataToCloud);
        }

        public void Add(PortfolioBllModel item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (storage.GetByPredicate(m => m.Symbol == item.Symbol && m.UserId == item.UserId).Count() > 0)
                throw new ArgumentException("Item with this symbol already exists", nameof(item));
            item.Status = DAL.DTO.SyncronizationStatus.New;
            storage.Add(item.ToDALModel());
        }


        public void Delete(int itemId)
        {
            var item = storage.GetById(itemId);
            item.Status = DAL.DTO.SyncronizationStatus.Deleted;
            storage.Update(item);
        }

        public IEnumerable<PortfolioBllModel> GetAll(int userId)
        {
            var result = storage.GetByPredicate(m => m.UserId == userId && m.Status != DAL.DTO.SyncronizationStatus.Deleted).Select(m => m.ToBLLModel());
            return result;
        }

        public void Update(PortfolioBllModel item)
        {
            if (storage.GetById(item.ItemId).Status == DAL.DTO.SyncronizationStatus.New)
            {
                item.Status = DAL.DTO.SyncronizationStatus.New;
                storage.Update(item.ToDALModel());
                return;
            }
            item.Status = DAL.DTO.SyncronizationStatus.Dirty;
            item.RemoteId = storage.GetById(item.ItemId).RemoteId;
            storage.Update(item.ToDALModel());
        }
        private void UploadDataToCloud()
        {
            while (true)
            {
                IEnumerable<PortfolioBllModel> itemsToSyncronize;
                itemsToSyncronize = storage.GetByPredicate(m => m.Status != DAL.DTO.SyncronizationStatus.Syncronized).Select(m => m.ToBLLModel());
                itemsToSyncronize.AsParallel().ForAll(item =>
                {
                    HttpResponseMessage response;
                    switch (item.Status)
                    {
                        case DAL.DTO.SyncronizationStatus.New:
                            response = _httpClient.PostAsJsonAsync(_serviceApiUrl + CreateUrl, new CloudDTO()
                            {
                                SharesNumber = item.SharesNumber,
                                Symbol = item.Symbol,
                                UserId = item.UserId
                            }).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                var remoteId = GetRemoteId(item);
                                var storedItem = storage.GetById(item.ItemId).ToBLLModel();
                                storedItem.Status = GetProperStatus(item, storedItem);
                                storedItem.RemoteId = remoteId;
                                storage.Update(storedItem.ToDALModel());
                            }
                            break;
                        case DAL.DTO.SyncronizationStatus.Dirty:
                            response = _httpClient.PutAsJsonAsync(_serviceApiUrl + UpdateUrl, new CloudDTO()
                            {
                                ItemId = item.RemoteId,
                                SharesNumber = item.SharesNumber,
                                Symbol = item.Symbol,
                                UserId = item.UserId
                            }).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                if (AreEqual(item, storage.GetById(item.ItemId).ToBLLModel()))
                                {
                                    item.Status = DAL.DTO.SyncronizationStatus.Syncronized;
                                    storage.Update(item.ToDALModel());
                                }
                            }
                            break;
                        case DAL.DTO.SyncronizationStatus.Deleted:
                            if (item.RemoteId == 0)
                            {
                                storage.Delete(item.ItemId);
                                break;
                            }
                            response = _httpClient.DeleteAsync(string.Format(_serviceApiUrl + DeleteUrl, item.RemoteId)).Result;
                            if (response.IsSuccessStatusCode)
                                storage.Delete(item.ItemId);
                            break;
                    }
                });
                Thread.Sleep(cloudUpdateTimeOut);
            }
        }

        private  int GetRemoteId(PortfolioBllModel item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.RemoteId > 0)
                return item.RemoteId;
            var dataAsString = _httpClient.GetStringAsync(string.Format(_serviceApiUrl + GetAllUrl, item.UserId)).Result;
            var userData = JsonConvert.DeserializeObject<IList<CloudDTO>>(dataAsString);
            return userData.FirstOrDefault(m => m.SharesNumber == item.SharesNumber && m.Symbol == item.Symbol).ItemId;
        }

        private bool AreEqual(PortfolioBllModel item, PortfolioBllModel storedItem)
        {
            return item.SharesNumber == storedItem.SharesNumber &&
                item.Symbol == storedItem.Symbol &&
                item.Status == storedItem.Status;
        }

        private DAL.DTO.SyncronizationStatus GetProperStatus(PortfolioBllModel item, PortfolioBllModel storedItem)
        {
            if (AreEqual(item, storedItem))
                return DAL.DTO.SyncronizationStatus.Syncronized;
            if (storedItem.Status == DAL.DTO.SyncronizationStatus.New)
                return DAL.DTO.SyncronizationStatus.Dirty;
            return storedItem.Status;
        }
    }
}
