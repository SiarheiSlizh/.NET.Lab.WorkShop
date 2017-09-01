using System.Collections.Generic;
using System.Web.Http;
using PortfolioManagerClient.Models;
using PortfolioManagerClient.Services;
using PortfolioManager.Service.Models;
using PortfolioManager.Service;
using PortfolioManager.Service.Interfaces;
using PortfolioManagerClient.Infrastructure;
using System.Linq;
using Ninject;
using Ninject.Web.Common;
using System;

namespace PortfolioManagerClient.Controllers
{
    /// <summary>
    /// Processes portfolio item requests.
    /// </summary>
    public class PortfolioItemsController : ApiController
    {
        public PortfolioItemsController(IService service)
        {
            this._portfolioItemsService = service;
        }
        private readonly IService _portfolioItemsService;
        private readonly UsersService _usersService = new UsersService();

        /// <summary>
        /// Returns all portfolio items for the current user.
        /// </summary>
        /// <returns>The list of portfolio items.</returns>
        public IList<PortfolioItemViewModel> Get()
        {
            var userId = _usersService.GetOrCreateUser();
            var result = _portfolioItemsService.GetAll(userId).Select(m => m.ToViewModel()).ToList();
            return result;
        }

        /// <summary>
        /// Updates the existing portfolio item.
        /// </summary>
        /// <param name="portfolioItem">The portfolio item to update.</param>
        public void Put(PortfolioItemViewModel portfolioItem)
        {
            portfolioItem.UserId = _usersService.GetOrCreateUser();
            _portfolioItemsService.Update(portfolioItem.ToBLLModel());
        }

        /// <summary>
        /// Deletes the specified portfolio item.
        /// </summary>
        /// <param name="id">The portfolio item identifier.</param>
        public void Delete(int id)
        {
            _portfolioItemsService.Delete(id);
        }

        /// <summary>
        /// Creates a new portfolio item.
        /// </summary>
        /// <param name="portfolioItem">The portfolio item to create.</param>
        public void Post(PortfolioItemViewModel portfolioItem)
        {
            try
            {
                portfolioItem.UserId = _usersService.GetOrCreateUser();
                _portfolioItemsService.Add(portfolioItem.ToBLLModel());
            }
            catch(ArgumentException e)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.Conflict);
            }
        }
    }
}
