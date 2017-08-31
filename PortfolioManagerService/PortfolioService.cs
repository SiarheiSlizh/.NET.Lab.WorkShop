﻿using System;
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
        private readonly Timer timer;
        public PortfolioService (IStorage storage)
        {
            this.storage = storage;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            timer = new Timer(UploadDataToCloud, null, 0, cloudUpdateTimeOut);
        }
        public void Add(PortfolioBllModel item)
        {
            item.Status = DAL.DTO.SyncronizationStatus.New;
            storage.Add(item.ToDALModel());
        }


        public void Delete(int itemId)
        {
            var item = storage.GetById(itemId);
            if (item.Status == DAL.DTO.SyncronizationStatus.New)
            {
                storage.Delete(itemId);
                return;
            }
            item.Status = DAL.DTO.SyncronizationStatus.Deleted;
            storage.Update(item);
        }

        //TODO use storage
        public IEnumerable<PortfolioBllModel> GetAll(int userId)
        {
            var result = storage.GetAll(userId).Select(m => m.ToBLLModel());
            return result;
        }

        public void Update(PortfolioBllModel item)
        {
            item.Status = DAL.DTO.SyncronizationStatus.Dirty;
            storage.Update(item.ToDALModel());
        }

        private void UploadDataToCloud(object state)
        {
            var itemsToSyncronize = storage.GetByPredicate(m => m.Status != DAL.DTO.SyncronizationStatus.Syncronized).Select(m => m.ToBLLModel());
            Parallel.ForEach(itemsToSyncronize, async item =>
            {
                HttpResponseMessage response;
                switch (item.Status)
                {
                    case DAL.DTO.SyncronizationStatus.New:
                        response = await _httpClient.PostAsJsonAsync(_serviceApiUrl + CreateUrl, item);
                        if (response.IsSuccessStatusCode)
                        { 
                            var remoteId = await GetRemoteId(item);
                            var storedItem = storage.GetById(item.ItemId).ToBLLModel();
                            storedItem.Status = GetProperStatus(item, storedItem);
                            storedItem.RemoteId = remoteId;
                            storage.Update(storedItem.ToDALModel());
                        }
                        break;
                    case DAL.DTO.SyncronizationStatus.Dirty:
                        response = await _httpClient.PutAsJsonAsync(_serviceApiUrl + UpdateUrl, item);
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
                        response = await _httpClient.DeleteAsync(string.Format(_serviceApiUrl + DeleteUrl, item.ItemId));
                        if (response.IsSuccessStatusCode)
                            storage.Delete(item.ItemId);
                        break;
                }
            });
        }

        private async Task<int> GetRemoteId(PortfolioBllModel item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.RemoteId > 0)
                return item.RemoteId;
            var dataAsString = await _httpClient.GetStringAsync(string.Format(_serviceApiUrl + GetAllUrl, item.UserId));
            var userData = JsonConvert.DeserializeObject<IList<CloudDTO>>(dataAsString);
            return userData.FirstOrDefault(m => m.SharesNumber == item.SharesNumber && m.Symbol == item.Symbol).ItemId;
        }

        private bool AreEqual(PortfolioBllModel item, PortfolioBllModel storedItem)
        {
            return item.SharesNumber == storedItem.SharesNumber ||
                item.Symbol == storedItem.Symbol ||
                item.Status == storedItem.Status;
        }

        private DAL.DTO.SyncronizationStatus GetProperStatus(PortfolioBllModel item, PortfolioBllModel storedItem)
        {
            if (AreEqual(item, storedItem))
                return DAL.DTO.SyncronizationStatus.Syncronized;
            if (storedItem.Status == DAL.DTO.SyncronizationStatus.Deleted)
                return DAL.DTO.SyncronizationStatus.Deleted;
            return DAL.DTO.SyncronizationStatus.Dirty;
        }
    }
}
