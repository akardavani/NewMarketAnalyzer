using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Application.Services;
using MarketAnalyzer.SharedKernel;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class PersonUpsertedNotificationHandler : INotificationHandler<PersonUpsertedNotification>
    {
        private readonly IMediator _mediator;
        private readonly PasswordGenerator _passwordGenerator;

        public PersonUpsertedNotificationHandler(IMediator mediator, PasswordGenerator passwordGenerator)
        {
            _mediator = mediator;
            _passwordGenerator = passwordGenerator;
        }

        public async Task HandleAsync(PersonUpsertedNotification notification, CancellationToken cancellationToken)
        {
            if (notification.CreatePortalUser)
            {
                var command = new CreateUserCommand()
                {
                    FullName = notification.Name,
                    UserName = notification.NationalCode,
                    Password = _passwordGenerator.Generate(8),
                    PhoneNumber = notification.PhoneNumber,
                    Email = GenerateEmail(notification.Email, notification.NationalCode),
                    EmailConfirmed = !string.IsNullOrEmpty(notification.Email),
                    TwoFactorAuthentication = true,
                    Roles = null,
                    ApplicationId = Domain.Application.ClientWeb.Id,
                    IpAddresses = null,
                    Claims =
                    [
                        new(key: JwtTokenPayloads.NationalCodeClaim, value: notification.NationalCode),
                        new(key: JwtTokenPayloads.RequestPermitClaim, value: notification.RequestPermit ? "1" : "0"),
                    ]
                };

                await _mediator.ExecuteAsync(command, cancellationToken);
            }
        }

        private static string GenerateEmail(string email, string nationalCode)
        {
            return string.IsNullOrEmpty(email) ?
                 $"{nationalCode}@puyaandishan.fund" :
                email;
        }
    }
}
