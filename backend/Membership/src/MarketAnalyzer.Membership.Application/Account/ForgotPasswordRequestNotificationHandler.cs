using MarketAnalyzer.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class ForgotPasswordRequestNotificationHandler : INotificationHandler<ForgotPasswordRequestNotification>
    {
        private readonly IMessageSender _messageSender;

        public ForgotPasswordRequestNotificationHandler(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public Task HandleAsync(ForgotPasswordRequestNotification notification, CancellationToken cancellationToken)
        {
            var lifetime = (int)notification.TokenLifetime.TotalMinutes;
            var smsTask = SendSmsAsync(notification.OtpCode, notification.PhoneNumber, lifetime, cancellationToken);
            var emailTask = !notification.EmailConfirmed ? 
                Task.CompletedTask : 
                SendEmailAsync(notification.OtpCode, notification.Email, lifetime, cancellationToken);
            return Task.WhenAll(smsTask, emailTask);
        }

        public Task SendSmsAsync(string otpCode, string phoneNumber, int lifetime, CancellationToken cancellationToken)
        {
            return _messageSender.SendAsync(new OutgoingMessage()
            {
                Subject = "فراموشی رمز عبور",
                MessageBody = $"کد بازنشانی رمز عبور\r\n{otpCode}\r\nمهلت استفاده {lifetime} دقیقه میباشد.",
                AttemptCount = 0,
                Recipient = phoneNumber,
                Type = OutgoingMessageType.Sms
            }, false, cancellationToken);
        }

        public Task SendEmailAsync(string otpCode, string email, int lifetime, CancellationToken cancellationToken)
        {
            return _messageSender.SendAsync(new OutgoingMessage()
            {
                Subject = "فراموشی رمز عبور",
                MessageBody = $"کد بازنشانی رمز عبور\r\n{otpCode}\r\nمهلت استفاده {lifetime} دقیقه میباشد.",
                AttemptCount = 0,
                Recipient = email,
                Type = OutgoingMessageType.Email
            }, false, cancellationToken);
        }
    }
}
