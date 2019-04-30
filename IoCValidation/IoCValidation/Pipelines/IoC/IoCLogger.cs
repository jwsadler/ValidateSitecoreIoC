using log4net;

namespace IoCValidation.Pipelines.IoC
{
    public class IoCLogger
    {
        private static ILog _log;

        public static ILog Log => _log ?? (_log = LogManager.GetLogger(typeof(IoCLogger)));
    }
}