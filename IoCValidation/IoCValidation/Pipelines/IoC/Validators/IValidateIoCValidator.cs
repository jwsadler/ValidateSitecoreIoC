using System;
using IoCValidation.Interfaces;

namespace IoCValidation.Pipelines.IoC.Validators
{
    public interface IValidateIoCValidator : IService
    {
        void Validate(object x, IoCMetaData result, Type serviceType);
    }
}