using System.Web.Http;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web;
using PortfolioManager.Service.Interfaces;
using PortfolioManager.Service;

namespace PortfolioManagerClient
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
