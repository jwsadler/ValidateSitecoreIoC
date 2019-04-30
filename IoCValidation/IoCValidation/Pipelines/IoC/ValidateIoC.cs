using System;
using System.Collections.Generic;
using System.Linq;
using IoCValidation.Pipelines.IoC.Validators;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.DependencyInjection;
using Sitecore.Pipelines;

// ReSharper disable UseStringInterpolation

// ReSharper disable UseNullPropagation
// ReSharper disable UsePatternMatching

namespace IoCValidation.Pipelines.IoC
{
    public class ValidateIoC
    {
        private const string CONTAINS_GENERIC_PARAMETER = "Type ContainsGenericParameters == true";

        private const string VALIDATE_IOC_ENABLED = "ValidateIoC.Enabled";

        private const string VALIDATE_IOC_HALT_APPLICATION = "ValidateIoC.HaltApplication";

        private const string VALIDATE_IOC_IGNORED_EXCEPTIONS = "ValidateIoC.IgnoredExceptions";

        public void Process(PipelineArgs args)
        {
            if (!Settings.GetBoolSetting(VALIDATE_IOC_ENABLED, true)) return;

            var serviceCollection = ServiceLocator.ServiceProvider.GetService<IServiceCollection>();
            if (serviceCollection == null) return;

            IoCLogger.Log.Info(string.Format("IoC Validation Started {0:g}", DateTime.Now));

            var result = new IoCMetaData();

            foreach (var service in serviceCollection)
            {
                var serviceType = service.ServiceType;
                if (result.ValidTypes.Any(p => p.ServiceType == serviceType.ToString()))
                {
                    continue;
                }

                try
                {
                    if (serviceType.ContainsGenericParameters)
                    {
                        result.NotValidatedTypes.Add(new ServiceMetaData { ServiceType = serviceType.ToString(), Reason = CONTAINS_GENERIC_PARAMETER });
                        continue;
                    }

                    var xs = ServiceLocator.ServiceProvider.GetServices(service.ServiceType);
                    foreach(var x in xs)
                    {
                        if (x == null) continue;
                        result.ValidTypes.Add(
                            new ValidServiceMetaData { ServiceType = serviceType.ToString(), ConcreteType = x.GetType().FullName});

                        //Run External Tests
                        var validators = ServiceLocator.ServiceProvider.GetServices<IValidateIoCValidator>();
                        foreach (var validator in validators) validator.Validate(x, result, serviceType);
                    }
                }
                catch (KeyNotFoundException)
                {
                    result.ValidTypes.Add(new ValidServiceMetaData { ServiceType = serviceType.ToString() });
                }
                catch (Exception e)
                {
                    result.Problems.Add(new ServiceMetaData { ServiceType = serviceType.ToString(), Reason = e.Message });
                }
            }

            var postProcessValidators = ServiceLocator.ServiceProvider.GetServices<IValidateIoCValidatorPostProcess>();
            foreach (var validator in postProcessValidators) validator.Validate(result);

            IoCLogger.Log.Info(JsonConvert.SerializeObject(result, Formatting.Indented));

            IoCLogger.Log.Info(string.Format("IoC Validation Finished {0:g}", DateTime.Now));

            if (Settings.GetBoolSetting(VALIDATE_IOC_HALT_APPLICATION, false))
            {
                var ignoredServices = Settings.GetSetting(VALIDATE_IOC_IGNORED_EXCEPTIONS, "").Split('|');
                if (result.Problems.Select(p => p.ServiceType).Except(ignoredServices).Any())
                {
                    args.AbortPipeline();
                    throw new ApplicationException("IoC Validation has failed and configuration is set to halt the application.  See IoC Logs for more info.");
                }
            }
        }
    }
}