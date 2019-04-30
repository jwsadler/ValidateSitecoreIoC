using System;
using System.Linq;
using IoCValidation.Interfaces;

namespace IoCValidation.Pipelines.IoC.Validators
{
    public class ValidateIHelperInterfaces : IValidateIoCValidator
    {
        private const string ERROR_MESSAGE = "One or More Parameters implements IService or IScoped";

        public void Validate(object x, IoCMetaData result, Type serviceType)
        {
            //Test to see if Singleton uses non-singleton services

            if (x is IHelper)
            {
                var foundServices = false;
                var constructors = x.GetType().GetConstructors();

                foreach (var constructor in constructors)
                {
                    var parameters = constructor.GetParameters();

                    foreach (var parameter in parameters)
                    {
                        if (parameter.ParameterType.GetInterfaces().Contains(typeof(IService)))
                        {
                            foundServices = true;
                            break;
                        }

                        if (foundServices) break;
                    }
                }

                if (foundServices) result.Warnings.Add(new ServiceMetaData { ServiceType = serviceType.ToString(), ConcreteType = x.GetType().FullName, Reason = ERROR_MESSAGE });
            }
        }
    }
}