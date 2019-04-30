using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace IoCValidation.Pipelines.IoC.Validators
{
    public class ValidateConstructorArguments : IValidateIoCValidator
    {
        private const string ERROR_MESSAGE = "One or More Parameters is not set up to be injected: {0}";

        public void Validate(object x, IoCMetaData result, Type serviceType)
        {
            var ignoredAssemblies = new[] { "System.Web", "System.Reflection", "CCA.CarpetOne.Data.MultiLocationManager", "Sitecore." };
            var assemblyQualifiedName = x.GetType().AssemblyQualifiedName;

            if (assemblyQualifiedName != null && ignoredAssemblies.Any(assemblyQualifiedName.Contains)) return;

            var someServicesNotInjected = false;

            //Get most overloaded constructor
            //Ignore Generic Repo
            var constructor = x.GetType().GetConstructors().Where(p => p.GetParameters().Any()).OrderByDescending(p => p.GetParameters().Length).FirstOrDefault();
            var serviceCollection = ServiceLocator.ServiceProvider.GetService<IServiceCollection>();

            var failedServices = new List<string>();
            if (constructor != null)
            {
                var parameters = constructor.GetParameters();
                foreach (var parameter in parameters)
                {
                    var parameterType = parameter.ParameterType;
                    if (typeof(IEnumerable).IsAssignableFrom(parameterType)) parameterType = parameter.ParameterType.GetGenericArguments()[0];

                    if (!parameterType.IsGenericType)
                        if (serviceCollection.All(p => p.ServiceType != parameterType))
                        {
                            someServicesNotInjected = true;
                            failedServices.Add(parameter.Name);
                        }
                }
            }

            if (someServicesNotInjected)
                result.Problems.Add(
                    new ServiceMetaData
                        {
                            ServiceType = serviceType.ToString(), ConcreteType = x.GetType().FullName, Reason = string.Format(ERROR_MESSAGE, string.Join(", ", failedServices))
                        });
        }
    }
}