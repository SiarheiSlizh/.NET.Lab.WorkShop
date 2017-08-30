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

        private readonly HttpClient _httpClient;
        private readonly IStorage storage;
        public PortfolioService (IStorage storage)
        {
            this.storage = storage;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }
        public void Add(PortfolioBllModel item)
        {
            
            //Add item to local storage
            storage.Add(item.ToDALModel());
            //Add Item to CloudService
            _httpClient.PostAsJsonAsync(_serviceApiUrl + CreateUrl, item);
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        //TODO use storage
        public IEnumerable<PortfolioBllModel> GetAll(int userId)
        {
            var result = storage.GetAll(userId).Select(m => m.ToBLLModel());
            return result;
        }

        public PortfolioBllModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(PortfolioBllModel item)
        {
            throw new NotImplementedException();
        }
    }
}
