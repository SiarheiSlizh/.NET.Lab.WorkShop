using PortfolioManager.Service.Models;
using PortfolioManagerClient.Models;
using System;

namespace PortfolioManagerClient.Infrastructure
{
    public static class Mapper
    {
        public static PortfolioItemViewModel ToViewModel(this PortfolioBllModel bllItem)
        {
            return new PortfolioItemViewModel()
            {
                ItemId = bllItem.ItemId,
                UserId = bllItem.UserId,
                SharesNumber = bllItem.SharesNumber,
                Symbol = bllItem.Symbol
            };
        }

        public static PortfolioBllModel ToBLLModel(this PortfolioItemViewModel vm)
        {
            return new PortfolioBllModel()
            {
                ItemId = vm.ItemId,
                UserId = vm.UserId,
                SharesNumber = vm.SharesNumber,
                Symbol = vm.Symbol
            };
        }
    }
}