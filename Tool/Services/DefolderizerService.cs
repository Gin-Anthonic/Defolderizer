using Defolderizer.Interfaces;
using Defolderizer.Models;
using System.Security;

namespace Defolderizer.Services;

public class DefolderizerService {

    private readonly ILoggingService _loggingService;
    private readonly IFeedbackService _feedbackService;

    public DefolderizerService(ILoggingService logger, IFeedbackService feedbackService) {
        _loggingService = logger;
        _feedbackService = feedbackService;
    }


    public void Execute(string path, string mode) {
        Console.WriteLine(_loggingService.WriteLogEntry("Current Directory: " + path + " Mode: " + mode, "Current Directory: [NAME REDACTED] Mode: " + mode));
        DirectoryInfo selectedDirectory = new DirectoryInfo(path);
        switch (mode) {
            case "unfold":
                Unfold(selectedDirectory);
                break;
            case "defolderize":
                Defolderize(selectedDirectory);
                break;
            case "recursive":
                ConfirmRecursiveDefolderize(selectedDirectory);
                break;
        }
    }


    public void ConfirmRecursiveDefolderize(DirectoryInfo selectedDirectory) {
        Console.WriteLine(_loggingService.WriteLogEntry("Recursively defolderizing directory " + selectedDirectory.FullName, "Recursively defolderizing directory [NAME REDACTED]"));

        DialogResult result = MessageBox.Show("You are about to recursively unfold the following directory: \n" + selectedDirectory.FullName + "\nProceed?", "Here be dragons!", MessageBoxButtons.YesNo);
        if (result == DialogResult.No) {
            Console.WriteLine(_loggingService.WriteLogEntry("Process was aborted by user"));
            return;
        }
        RecusriveDefolderize(selectedDirectory);
    }


    private void Defolderize(DirectoryInfo selectedDirectory) {
        Console.WriteLine(_loggingService.WriteLogEntry("Defolderizing directory " + selectedDirectory.FullName, "Defolderizing directory [NAME REDACTED]"));
        foreach (DirectoryInfo directory in selectedDirectory.GetDirectories()) {
            Unfold(directory);
        }
    }

    /*  Reminder for Future-Smooth-Brain-Gin: 
        Instead of repeatingly defolderizing the parent dir 
        this recusrively goes into the current dir until it reaches
        a dir with no subdirs and then unfolds from inside out
        so basilc depth-first defolderizing
    */
    private void RecusriveDefolderize(DirectoryInfo selectedDirectory) {

        foreach (DirectoryInfo directory in selectedDirectory.GetDirectories()) {
            RecusriveDefolderize(directory);
            Unfold(directory);
        }
    }


    private void Unfold(DirectoryInfo selectedDirectory) {
        Console.WriteLine(_loggingService.WriteLogEntry("Unfolding directory " + selectedDirectory.FullName, "Unfolding directory [NAME REDACTED]"));

        if (selectedDirectory.Parent == null) {
            Console.WriteLine(_loggingService.WriteLogEntry("The directory " + selectedDirectory.Name + " seems to have no parent, unfolding not possible... Exiting...", "The directory " + "[NAME REDACTED]" + " seems to have no parent, unfolding not possible... Exiting..."));
            return;
        }

        DirectoryInfo parentDirectory = selectedDirectory.Parent;
        MoveFiles(selectedDirectory, parentDirectory);
        MoveDirectories(selectedDirectory, parentDirectory);

        if (Directory.EnumerateFileSystemEntries(selectedDirectory.FullName).Count() != 0) {
            Console.WriteLine(_loggingService.WriteLogEntry("Directory is not Empty... Removal Failed."));
            _feedbackService.AddMessage("\nThe Directory " + selectedDirectory.Name + " was not removed as it still has contents!\n");
            return;
        }

        try {
            selectedDirectory.Delete();
        }
        catch (IOException e) {
            Console.WriteLine(_loggingService.WriteLogEntry("Removing the Directory  " + selectedDirectory.Name + " failed because:", "Removing the Directory  " + "[NAME REDACTED]" + " failed because:"));
            Console.WriteLine(_loggingService.WriteLogEntry(e.Message, e.ToString()));
            _feedbackService.AddMessage("\nAttempt to remove directory \"" + selectedDirectory.Name + "\" failed due to the following Exception: \n" + e.Message + "\n");
        }
        catch (UnauthorizedAccessException e) {
            Console.WriteLine(_loggingService.WriteLogEntry("Removing the Directory  " + selectedDirectory.Name + " failed because:", "Removing the Directory  " + "[NAME REDACTED]" + " failed because:"));
            Console.WriteLine(_loggingService.WriteLogEntry(e.Message, e.ToString()));
            _feedbackService.AddMessage("\nAttempt to remove directory \"" + selectedDirectory.Name + "\" failed due to the following Exception: \n" + e.Message + "\n");
        }
    }


    private void MoveFiles(DirectoryInfo selectedDirectory, DirectoryInfo parentDirectory) {
        FileInfo[] files = selectedDirectory.GetFiles();
        foreach (FileInfo file in files) {
            Console.WriteLine(_loggingService.WriteLogEntry("Current File: " + file.Name, "Current File: " + "[NAME REDACTED]"));

            string newFilePath = Path.Combine(parentDirectory.FullName, file.Name);
            string newFileName = file.Name;

            if (File.Exists(newFilePath)) {
                Console.WriteLine(_loggingService.WriteLogEntry("File Already Exists..."));
                newFileName = FindViableFileName(file, parentDirectory);
                newFilePath = Path.Combine(parentDirectory.FullName, newFileName);
            }

            Console.WriteLine(_loggingService.WriteLogEntry("Moving " + newFileName + "...", "Moving " + "[NAME REDACTED]" + "..."));

            try {
                file.MoveTo(newFilePath);
            }
            catch (IOException e) {
                Console.WriteLine(_loggingService.WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(_loggingService.WriteLogEntry(e.Message, e.ToString()));
                _feedbackService.AddMoveFailure(new MoveFailure(file, e));
            }
            catch (SecurityException e) {
                Console.WriteLine(_loggingService.WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(_loggingService.WriteLogEntry(e.Message, e.ToString()));
                _feedbackService.AddMoveFailure(new MoveFailure(file, e));
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(_loggingService.WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(_loggingService.WriteLogEntry(e.Message, e.ToString()));
                _feedbackService.AddMoveFailure(new MoveFailure(file, e));
            }
        }
    }


    private void MoveDirectories(DirectoryInfo selectedDirectory, DirectoryInfo parentDirectory) {
        foreach (DirectoryInfo directory in selectedDirectory.GetDirectories()) {
            Console.WriteLine(_loggingService.WriteLogEntry("Current Directory: " + directory.Name, "Current Directory: " + "[NAME REDACTED]"));

            string newDirectoryPath = Path.Combine(parentDirectory.FullName, directory.Name);
            string newDirectoryName = directory.Name;

            if (Directory.Exists(newDirectoryPath)) {
                Console.WriteLine(_loggingService.WriteLogEntry("Directory already exists..."));
                newDirectoryName = FindViableDirectoryName(directory, parentDirectory);
                newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);
            }

            Console.WriteLine(_loggingService.WriteLogEntry("Moving directory " + newDirectoryName + "...", "Moving directory " + "[NAME REDACTED]" + "..."));

            try {

                directory.MoveTo(newDirectoryPath);
            }
            catch (IOException e) {
                Console.WriteLine(_loggingService.WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(_loggingService.WriteLogEntry(e.Message, e.ToString()));
                _feedbackService.AddMoveFailure(new MoveFailure(directory, e));
            }
            catch (SecurityException e) {
                Console.WriteLine(_loggingService.WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(_loggingService.WriteLogEntry(e.Message, e.ToString()));
                _feedbackService.AddMoveFailure(new MoveFailure(directory, e));
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(_loggingService.WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(_loggingService.WriteLogEntry(e.Message, e.ToString()));
                _feedbackService.AddMoveFailure(new MoveFailure(directory, e));
            }
        }
    }


    private string FindViableFileName(FileInfo file, DirectoryInfo parentDirectory) {
        Console.WriteLine(_loggingService.WriteLogEntry("Finding new name..."));

        string extenstionlessFileName = file.Name[..file.Name.LastIndexOf(".")];
        string newFilePath = Path.Combine(parentDirectory.FullName, file.Name);
        int copyCounter = 0;
        string newFileName = "";

        while (File.Exists(newFilePath)) {
            copyCounter++;
            newFileName = extenstionlessFileName + "_copy" + copyCounter + file.Extension;
            newFilePath = Path.Combine(parentDirectory.FullName, newFileName);

            Console.WriteLine(_loggingService.WriteLogEntry("Checking name " + newFileName + "...", "Checking name " + "[NAME REDACTED]..."));
        }

        Console.WriteLine(_loggingService.WriteLogEntry("New Name: " + newFileName, "New Name: " + "[NAME REDACTED]"));

        return (newFileName);
    }


    private string FindViableDirectoryName(DirectoryInfo directory, DirectoryInfo parentDirectory) {
        Console.WriteLine(_loggingService.WriteLogEntry("Finding new name..."));

        string newDirectoryPath = Path.Combine(parentDirectory.FullName, directory.Name);
        int copyCounter = 1;
        string newDirectoryName = "";

        while (Directory.Exists(newDirectoryPath)) {
            copyCounter++;
            newDirectoryName = directory.Name + "_copy" + copyCounter;
            newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);

            Console.WriteLine(_loggingService.WriteLogEntry("Checking name " + newDirectoryName + "...", "Checing name [NAME REDACTED]..."));
        }
        Console.WriteLine(_loggingService.WriteLogEntry("New Name: " + newDirectoryName, "New Name: " + "[NAME REDACTED]"));

        return (newDirectoryName);
    }
}
