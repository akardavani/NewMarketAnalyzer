using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    public class CreateNewsCategoryCommandHandler : IRequestHandler<CreateNewsCategoryCommand, ApiResult<int>>
    {
        private readonly IDbContext _dbContext;

        public CreateNewsCategoryCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResult<int>> HandleAsync(CreateNewsCategoryCommand request, CancellationToken cancellationToken = default)
        {
            var exists = await _dbContext
                .Set<NewsCategory>()
                .AnyAsync(e => e.Name == request.Name, cancellationToken);

            if (exists)
            {
                return ApiResult.FromError<int>($" دسته بندی خبر' {request.Name}' قبلا ثبت شده است.");
            }

            var newsCategory = NewsCategory.Create(
                    name: request.Name,
                    description: request.Description);

            _dbContext
                .Set<NewsCategory>()
                .Add(newsCategory);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return ApiResult.From(newsCategory.Id);
        }
    }
}
