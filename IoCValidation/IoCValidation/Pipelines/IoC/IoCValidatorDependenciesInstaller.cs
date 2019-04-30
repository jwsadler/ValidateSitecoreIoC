using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace IoCValidation.Pipelines.IoC
{
    public class IoCValidatorDependenciesInstaller : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(p => serviceCollection);
        }
    }
}