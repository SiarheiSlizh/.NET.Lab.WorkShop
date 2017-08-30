using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioManager.Service;
using PortfolioManager.Service.Interfaces;
using DAL.Interfaces;
using DAL.Repositories;
using Ninject;

namespace DIConfig
{
    public static class DIConfig
    {
        public static void Configure(IKernel kernel, string storagePath)
        {
            kernel.Bind<IService>().To<PortfolioService>();
            kernel.Bind<IStorage>().To<XmlLocalStorage>().WithConstructorArgument("path", storagePath);
        }
    }
}
