using Microsoft.Practices.Unity;
using Ranken.ISC.Contracts.Repositories;
using RankenData.InterfacesSAPCognos.Domain;
using RankenData.InterfacesSAPCognos.Model.Repositories;
using System.Web.Http;
using Unity.WebApi;

namespace RankenData.InterfacesSAPCognos.WebAPI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            container.RegisterType<IRepository<CuentaCognos>, CuentaCognosRepository>();
        }
    }
}