using DAL.DTO;
using PortfolioManager.Service.Models;
using System;

namespace PortfolioManager.Service.Infrastructure
{
    public static class Mapper
    {
        public static PortfolioItemDAL ToDALModel(this PortfolioBllModel bllItem)
        {
            return new PortfolioItemDAL()
            {
                ItemId = bllItem.ItemId,
                UserId = bllItem.UserId,
                SharesNumber = bllItem.SharesNumber,
                Symbol = bllItem.Symbol,
                RemoteId = bllItem.RemoteId,
                Status = bllItem.Status
            };
        }

        public static PortfolioBllModel ToBLLModel(this PortfolioItemDAL dalModel)
        {
            return new PortfolioBllModel()
            {
                ItemId = dalModel.ItemId,
                UserId = dalModel.UserId,
                SharesNumber = dalModel.SharesNumber,
                Symbol = dalModel.Symbol,
                RemoteId = dalModel.RemoteId,
                Status = dalModel.Status
            };
        }
    }
}
