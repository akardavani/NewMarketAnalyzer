using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using MarketAnalyzer.Membership.Domain.Notifications;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class UserPasswordResetedNotificationHandler : INotificationHandler<UserPasswordResetedNotification>
    {
        private readonly IMessageSender _messageSender;

        public UserPasswordResetedNotificationHandler(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public Task HandleAsync(UserPasswordResetedNotification notification, CancellationToken cancellationToken)
        {
            var smsTask = SendSmsAsync(notification, cancellationToken);
            var emailTask = !notification.EmailConfirmed ?
                Task.CompletedTask : 
                SendEmailAsync(notification, cancellationToken);
            return Task.WhenAll(smsTask, emailTask);
        }

        private Task SendEmailAsync(UserPasswordResetedNotification notification, CancellationToken cancellationToken)
        {

            var body = $"nاطلاعات کاربری شما در صندوق مشترک مبین سرمایه\r\nنام کاربری: {notification.UserName}\r\nرمز عبور: {notification.Password}";

            return _messageSender.SendAsync(new OutgoingMessage()
            {
                Subject = "حساب کاربری صندوق مشترک مبین سرمایه",
                MessageBody = body,
                AttemptCount = 0,
                Recipient = notification.Email,
                Type = OutgoingMessageType.Email
            }, false, cancellationToken);
        }

        private Task SendSmsAsync(UserPasswordResetedNotification notification, CancellationToken cancellationToken)
        {
            var body = $"صندوق مشترک مبین سرمایه\r\nاطلاعات کاربری شما\r\nنام کاربری: {notification.UserName}\r\nرمز عبور: {notification.Password}";
            return _messageSender.SendAsync(new OutgoingMessage()
            {
                Subject = "حساب کاربری صندوق مشترک مبین سرمایه",
                MessageBody = body,
                AttemptCount = 0,
                Recipient = notification.PhoneNumber,
                Type = OutgoingMessageType.Sms
            }, false, cancellationToken);
        }
    }
}
