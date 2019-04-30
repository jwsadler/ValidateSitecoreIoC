using IoCValidation.Interfaces;

namespace IoCValidation.Pipelines.IoC.Validators
{
    public interface IValidateIoCValidatorPostProcess : IService
    {
        void Validate( IoCMetaData result);
    }
}