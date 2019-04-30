using System.Collections.Generic;

namespace IoCValidation.Pipelines.IoC
{
    public class IoCMetaData
    {
        public IoCMetaData()
        {
            NotValidatedTypes = new List<ServiceMetaData>();
            Problems = new List<ServiceMetaData>();
            ValidTypes = new List<ValidServiceMetaData>();
            Warnings = new List<ServiceMetaData>();
            Messages= new List<string>();
        }

        public List<ServiceMetaData> NotValidatedTypes { get; set; }

        public List<ServiceMetaData> Problems { get; set; }

        public List<ValidServiceMetaData> ValidTypes { get; set; }

        public List<string> Messages { get; set; }

        public List<ServiceMetaData> Warnings { get; set; }
    }

    public class ServiceMetaData
    {
        public string Reason { get; set; }

        public string ServiceType { get; set; }

        public string ConcreteType { get; set; }
    }

    public class ValidServiceMetaData
    {
        public string ServiceType { get; set; }

        public string ConcreteType { get; set; }
    }
}