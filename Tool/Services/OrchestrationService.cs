using Defolderizer.Interfaces;

namespace Defolderizer.Services;

internal class OrchestrationService {

    private readonly ArgValidationService _argValidationService;
    private readonly ILoggingService _loggingService;
    private readonly IFeedbackService _feedbackService;
    private readonly DefolderizerService _defolderizerService;

    public OrchestrationService(ArgValidationService argValidationService, ILoggingService loggingService, IFeedbackService feedbackService, DefolderizerService defolderizerService ) {
        _argValidationService = argValidationService;
        _loggingService = loggingService;
        _feedbackService = feedbackService;
        _defolderizerService = defolderizerService;
    }

    public void Execute(string[] args) {
        Console.WriteLine(_loggingService.WriteLogEntry("-----------------Program Started-----------------"));

        ArgValidationService.ValidationResult result = _argValidationService.ValidateArgs(args);
        if (result != ArgValidationService.ValidationResult.Ok) {
            Console.WriteLine(_loggingService.WriteLogEntry(_argValidationService.ResultMessages[result]));
            return;
        }

        string currentDirectoryPath = args[0];
        string mode = args[1];

        _defolderizerService.Execute(currentDirectoryPath, mode);
        _feedbackService.ShowUserFeedback();
    }

}
