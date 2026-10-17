using MarketAnalyzer.Abstractions;
using MarketAnalyzer.SharedKernel;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class UserCreatedNotificationHandler : INotificationHandler<UserCreatedNotification>
    {
        private readonly IMessageSender _messageSender;

        public UserCreatedNotificationHandler(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public Task HandleAsync(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            var smsTask = SendSmsAsync(notification, cancellationToken);
            var emailTask = !notification.EmailConfirmed ? 
                Task.CompletedTask : 
                SendEmailAsync(notification, cancellationToken);
            return Task.WhenAll(smsTask, emailTask);
        }

        private Task SendEmailAsync(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            var body = $"مشخصات حساب کاربری شما در صندوق مشترک مبین سرمایه\r\nنام کاربری: {notification.UserName}\r\nرمز عبور: \u202A{notification.Password}\u202C";

            return _messageSender.SendAsync(new OutgoingMessage()
            {
                Subject = "حساب کاربری صندوق مشترک مبین سرمایه",
                MessageBody = body,
                AttemptCount = 0,
                Recipient = notification.Email,
                Type = OutgoingMessageType.Email
            }, false, cancellationToken);
        }

        private Task SendSmsAsync(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            var body = $"صندوق مشترک مبین سرمایه\r\nنام کاربری: {notification.UserName}\r\nرمز عبور: {notification.Password}";
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
