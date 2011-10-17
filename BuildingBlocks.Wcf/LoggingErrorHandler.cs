using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Common.Logging;

namespace BuildingBlocks.Wcf
{
    public class LoggingErrorHandler : IErrorHandler
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }

        public bool HandleError(Exception error)
        {
            _logger.Error(m => m("Unhandeled error"), error);
            return false;
        }
    }

    public class LoggingErrorHandler<TService> : IErrorHandler
    {
        private readonly ILog _logger = LogManager.GetLogger<TService>();

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }

        public bool HandleError(Exception error)
        {
            _logger.Error(m => m("Unhandeled error"), error);
            return false;
        }
    }
}