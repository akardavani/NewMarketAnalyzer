using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    internal class CreateContactUsCommandHandler : IRequestHandler<CreateContactUsCommand, int>
    {
        private readonly IDbContext _dbContext;

        public CreateContactUsCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> HandleAsync(CreateContactUsCommand request, CancellationToken cancellationToken = default)
        {
            var contact = ContactUs.Create(
                fullName: request.FullName,
                email: request.Email,
                phoneNumber: request.PhoneNumber,
                subject: request.Subject,
                text: request.Text,
                ipAddress: request.IpAddress);

            _dbContext.Set<ContactUs>().Add(contact);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return contact.Id;
        }
    }
}
