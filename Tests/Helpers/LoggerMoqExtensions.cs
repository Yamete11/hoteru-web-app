using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace hoteru_be.Tests.Helpers
{
    public static class LoggerMoqExtensions
    {
        public static void VerifyLogLevel<T>(this Mock<ILogger<T>> logger, LogLevel level, Times times)
        {
            logger.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((_, __) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((_, __) => true)),
                times);
        }
    }
}
