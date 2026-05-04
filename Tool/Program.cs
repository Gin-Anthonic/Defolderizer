using Microsoft.VisualBasic.FileIO;

namespace Defolderizer;

internal class Program {

    static void Main(string[] args) {

        //SetupTestFolder();
        Console.ReadKey();
        Console.WriteLine("Has been run with the following arguments: " + args[0] + ", " + args[1]);
        Console.ReadKey();
        //return;

        LoggingService logger = new LoggingService();

        if (ValidateArgs(args,logger) == false) return;

        string currentDirectoryPath = args[0];
        string mode = args[1];

        DirectoryInfo currentDirectory = new DirectoryInfo(currentDirectoryPath);

        DefolderizerService defolderizer = new DefolderizerService(currentDirectory,logger);

        Console.WriteLine(logger.WriteLogEntry("-----------------Program Started-----------------"));
        Console.WriteLine(logger.WriteLogEntry("Current Directory: " + currentDirectoryPath + " Mode: " + mode, "Current Directory: [NAME REDACTED] Mode: " + mode));

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

        logger.ShowUserFeedbackPopup();
        logger.Close();
        Console.ReadKey();
    }


    private static bool ValidateArgs(string[] args, LoggingService logger) {
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


    private static void SetupTestFolder() {
        Directory.Delete("C:\\Users\\Work\\Desktop\\testing", true);
        Directory.CreateDirectory("C:\\Users\\Work\\Desktop\\testing");
        FileSystem.CopyDirectory("C:\\Users\\Work\\Documents\\gin", "C:\\Users\\Work\\Desktop\\testing\\gin");
    }

}
