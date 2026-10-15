using MarketAnalyzer.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class OtpRequestNotificationHandler : INotificationHandler<OtpRequestNotification>
    {
        private readonly IMessageSender _messageSender;
        private readonly IOtpAuthenticationService _otpAuthenticationService;

        public OtpRequestNotificationHandler(IMessageSender messageSender, IOtpAuthenticationService otpAuthenticationService)
        {
            _messageSender = messageSender;
            _otpAuthenticationService = otpAuthenticationService;
        }

        public Task HandleAsync(OtpRequestNotification notification, CancellationToken cancellationToken)
        {
            var lifetime = (int)notification.TokenLifetime.TotalMinutes;
            var otpCode = _otpAuthenticationService.GenerateCode(notification.TokenLifetime, notification.SecurityStamp, notification.UserId);
            var smsTask = SendSmsAsync(otpCode, notification.PhoneNumber, lifetime, cancellationToken);
            var emailTask = !notification.EmailConfirmed ?
                Task.CompletedTask :
                SendEmailAsync(otpCode, notification.Email, lifetime, cancellationToken);
            return Task.WhenAll(smsTask, emailTask);
        }

        private Task SendEmailAsync(int otpCode, string email, int lifetime, CancellationToken cancellationToken)
        {
            return _messageSender.SendAsync(new OutgoingMessage()
            {
                Subject = "رمز عبور دوعاملی",
                MessageBody = $"رمز ورود به مدیریت صندوق\r\n{otpCode}\r\nمهلت استفاده {lifetime} دقیقه میباشد.",
                AttemptCount = 0,
                Recipient = email,
                Type = OutgoingMessageType.Email
            }, false, cancellationToken);
        }

        private Task SendSmsAsync(int otpCode, string phoneNumber, int lifetime, CancellationToken cancellationToken)
        {
            return _messageSender.SendAsync(new OutgoingMessage()
            {
                Subject = "رمز عبور دوعاملی",
                MessageBody = $"صندوق مبین سرمایه\n {otpCode}\n مهلت استفاده {lifetime} دقیقه",
                AttemptCount = 0,
                Recipient = phoneNumber,
                Type = OutgoingMessageType.Sms
            }, false, cancellationToken);
        }
    }
}
