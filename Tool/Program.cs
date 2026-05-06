using Defolderizer.Interfaces;
using Defolderizer.Services;
using Microsoft.VisualBasic.FileIO;

namespace Defolderizer;

internal class Program {

    static void Main(string[] args) {

        if (args.Length == 0) {
            Console.WriteLine("No Arguments given.. This tool is supposed to be run from the Context Menu...");
            Console.ReadKey();
            return;
        }

        ILoggingService loggingService = new FileLoggingService();
        IFeedbackService feedbackService = new FeedbackService();

        if (ValidateArgs(args,loggingService) == false) return;

        string currentDirectoryPath = args[0];
        string mode = args[1];

        DirectoryInfo currentDirectory = new DirectoryInfo(currentDirectoryPath);

        DefolderizerService defolderizer = new DefolderizerService(currentDirectory,loggingService,feedbackService);

        Console.WriteLine(loggingService.WriteLogEntry("-----------------Program Started-----------------"));
        Console.WriteLine(loggingService.WriteLogEntry("Current Directory: " + currentDirectoryPath + " Mode: " + mode, "Current Directory: [NAME REDACTED] Mode: " + mode));

        switch (mode) {
            case "unfold":
                defolderizer.Unfold();
                break;
            case "defolderize":
                defolderizer.Defolderize();
                break;
            case "recursive":
                defolderizer.RecursiveDefolderize();
                break;
        }

        feedbackService.ShowUserFeedback();
        loggingService.Close();
    }


    private static bool ValidateArgs(string[] args, ILoggingService logger) {
        if (args.Length != 2) {
            Console.WriteLine(logger.WriteLogEntry("Invalid number of Arguments given(path,mode).. exiting..."));
            return false;
        }

        if (!Directory.Exists(args[0])) {
            Console.WriteLine(logger.WriteLogEntry("Directory specified could not be found... exiting"));
            return false;
        }

        string[] validArgs = ["unfold", "defolderize", "recursive"];
        if (!validArgs.Contains(args[1])) {
            Console.WriteLine(logger.WriteLogEntry("Invalid argument for mode... exiting"));
            return false;
        }
        return true;
    }

}

