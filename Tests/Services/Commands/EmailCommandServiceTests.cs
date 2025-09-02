using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using hoteru_be.Services.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace hoteru_be.Tests.Services.Commands
{
    public class EmailCommandServiceTests
    {
        [Fact]
        public async Task SendEmailAsync_ShouldLogError_AndThrow_OnSmtpFailure()
        {
            var cfg = new Mock<IConfiguration>();
            cfg.SetupGet(c => c["EmailSettings:Email"]).Returns("from@test.com");
            cfg.SetupGet(c => c["EmailSettings:Password"]).Returns("pwd");
            cfg.SetupGet(c => c["EmailSettings:SmtpServer"]).Returns("invalid.invalid");
            cfg.SetupGet(c => c["EmailSettings:SmtpPort"]).Returns("2525");

            var logger = new Mock<ILogger<EmailCommandService>>();
            var sut = new EmailCommandService(cfg.Object, logger.Object);

            Func<Task> act = () => sut.SendEmailAsync("to@test.com", "subj", "body", CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();

            logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) =>
                        v.ToString()!.Contains("Failed to send email to") &&
                        v.ToString()!.Contains("to@test.com") &&
                        v.ToString()!.Contains("subj")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SendEmailAsync_ShouldThrowFormatException_WhenPortIsInvalid()
        {
            var cfg = new Mock<IConfiguration>();
            cfg.SetupGet(c => c["EmailSettings:Email"]).Returns("from@test.com");
            cfg.SetupGet(c => c["EmailSettings:Password"]).Returns("pwd");
            cfg.SetupGet(c => c["EmailSettings:SmtpServer"]).Returns("localhost");
            cfg.SetupGet(c => c["EmailSettings:SmtpPort"]).Returns("not-a-number");

            var logger = new Mock<ILogger<EmailCommandService>>();
            var sut = new EmailCommandService(cfg.Object, logger.Object);

            Func<Task> act = () => sut.SendEmailAsync("to@test.com", "subj", "body", CancellationToken.None);

            await act.Should().ThrowAsync<FormatException>();

            logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }
    }
}
