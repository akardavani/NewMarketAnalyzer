using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.Learn.Application;

public static class LearnApplicationRegistration
{
    public static IServiceCollection AddLearnApplication(this IServiceCollection services)
    {
        services.AddSingleton<ILearningContentImporter, HtmlQuestionImporter>();
        services.AddSingleton<ILearningImportSanitizer, LearningImportSanitizer>();
        services.AddSingleton<ILearningReviewPolicy, SpacedLearningReviewPolicy>();
        services.AddSingleton<ILearningExportRenderer, LearningHtmlExportRenderer>();
        return services;
    }
}
