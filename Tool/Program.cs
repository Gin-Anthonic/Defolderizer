using Defolderizer.Interfaces;
using Defolderizer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Defolderizer;

internal class Program {

    static void Main(string[] args) {

        ServiceCollection services = new ServiceCollection();

        services.AddTransient<OrchestrationService>();
        services.AddTransient<ArgValidationService>();
        services.AddTransient<DefolderizerService>();
        services.AddSingleton<ILoggingService, FileLoggingService>();
        services.AddSingleton<IFeedbackService, FeedbackService>();

        using (ServiceProvider serviceProvider = services.BuildServiceProvider()) {
            OrchestrationService app = serviceProvider.GetRequiredService<OrchestrationService>();
            app.Execute(args);
        }
    }
}

