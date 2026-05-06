using System.Security;

namespace Defolderizer;

public class DefolderizerService {

    private readonly LoggingService _logger;
    private readonly DirectoryInfo _selectedDirectory;
    private readonly FeedbackService _feedbackService;

    public DefolderizerService(DirectoryInfo givenDirectory, LoggingService logger,FeedbackService feedbackService) {
        _logger = logger;
        _selectedDirectory = givenDirectory;
        _feedbackService = feedbackService;
    }



    public void Unfold() {
        this.Unfold(_selectedDirectory);
    }


    public void Defolderize() {
        this.Defolderize(_selectedDirectory);
    }


    public void RecursiveDefolderize() {
        Console.WriteLine(_logger.WriteLogEntry("Recursively defolderizing directory " + _selectedDirectory.FullName, "Recursively defolderizing directory [NAME REDACTED]"));

        DialogResult result = MessageBox.Show("You are about to recursively unfold the following directory: \n" + _selectedDirectory.FullName + "\nProceed?", "Here be dragons!", MessageBoxButtons.YesNo);
        if (result == DialogResult.No) {
            Console.WriteLine(_logger.WriteLogEntry("Process was aborted by user"));
            return;
        }
        RecusriveDefolderize(_selectedDirectory);
    }



    private void Defolderize(DirectoryInfo currentDirectory) {
        Console.WriteLine(_logger.WriteLogEntry("Defolderizing directory " + currentDirectory.FullName, "Defolderizing directory [NAME REDACTED]"));
        foreach (DirectoryInfo directory in currentDirectory.GetDirectories()) {
            Unfold(directory);
        }
    }


    /*  Reminder for Future-Smooth-Brain-Gin: 
        Instead of repeatingly defolderizing the parent dir 
        this recusrively goes into the current dir until it reaches
        a dir with no subdirs and then unfolds from inside out
        so basilc depth-first defolderizing
    */
    private void RecusriveDefolderize(DirectoryInfo currentDirectory) {

        foreach (DirectoryInfo directory in currentDirectory.GetDirectories()) {
            RecusriveDefolderize(directory);
            Unfold(directory);
        }
    }


    private void Unfold(DirectoryInfo currentDirectory) {
        Console.WriteLine(_logger.WriteLogEntry("Unfolding directory " + currentDirectory.FullName, "Unfolding directory [NAME REDACTED]"));

        if (currentDirectory.Parent == null) {
            Console.WriteLine(_logger.WriteLogEntry("The directory " + currentDirectory.Name + " seems to have no parent, unfolding not possible... Exiting...", "The directory " + "[NAME REDACTED]" + " seems to have no parent, unfolding not possible... Exiting..."));
            return;
        }

        DirectoryInfo parentDirectory = currentDirectory.Parent;
        MoveFiles(currentDirectory, parentDirectory);
        MoveDirectories(currentDirectory, parentDirectory);

        if (Directory.EnumerateFileSystemEntries(currentDirectory.FullName).Count() != 0) {
            Console.WriteLine(_logger.WriteLogEntry("Directory is not Empty... Removal Failed."));
            _feedbackService.AddMessage("\nThe Directory " + currentDirectory.Name + " was not removed as it still has contents!\n");
            return;
        }

        try {
            currentDirectory.Delete();
        }
        catch (IOException e) {
            Console.WriteLine(_logger.WriteLogEntry("Removing the Directory  " + currentDirectory.Name + " failed because:", "Removing the Directory  " + "[NAME REDACTED]" + " failed because:"));
            Console.WriteLine(_logger.WriteLogEntry(e.Message, e.ToString()));
            _feedbackService.AddMessage("\nAttempt to remove directory \"" + currentDirectory.Name + "\" failed due to the following Exception: \n" + e.Message + "\n");
        }
        catch (UnauthorizedAccessException e) {
            Console.WriteLine(_logger.WriteLogEntry("Removing the Directory  " + currentDirectory.Name + " failed because:", "Removing the Directory  " + "[NAME REDACTED]" + " failed because:"));
            Console.WriteLine(_logger.WriteLogEntry(e.Message, e.ToString()));
            _feedbackService.AddMessage("\nAttempt to remove directory \"" + currentDirectory.Name + "\" failed due to the following Exception: \n" + e.Message + "\n");
        }
    }


    private void MoveFiles(DirectoryInfo currentDirectory, DirectoryInfo parentDirectory) {
        FileInfo[] files = currentDirectory.GetFiles();
        foreach (FileInfo file in files) {
            Console.WriteLine(_logger.WriteLogEntry("Current File: " + file.Name, "Current File: " + "[NAME REDACTED]"));

            string newFilePath = Path.Combine(parentDirectory.FullName, file.Name);
            string newFileName = file.Name;

            if (File.Exists(newFilePath)) {
                Console.WriteLine(_logger.WriteLogEntry("File Already Exists..."));
                newFileName = FindViableFileName(file, parentDirectory);
                newFilePath = Path.Combine(parentDirectory.FullName, newFileName);
            }

            Console.WriteLine(_logger.WriteLogEntry("Moving " + newFileName + "...", "Moving " + "[NAME REDACTED]" + "..."));

            try {
                file.MoveTo(newFilePath);
            }
            catch (IOException e) {
                Console.WriteLine(_logger.WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(_logger.WriteLogEntry(e.Message, e.ToString()));
                _feedbackService.AddMoveFailure(new MoveFailure(file, e));
            }
            catch (SecurityException e) {
                Console.WriteLine(_logger.WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(_logger.WriteLogEntry(e.Message, e.ToString()));
                _feedbackService.AddMoveFailure(new MoveFailure(file, e));
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(_logger.WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(_logger.WriteLogEntry(e.Message, e.ToString()));
                _feedbackService.AddMoveFailure(new MoveFailure(file, e));
            }
        }
    }


    private void MoveDirectories(DirectoryInfo currentDirectory, DirectoryInfo parentDirectory) {
        foreach (DirectoryInfo directory in currentDirectory.GetDirectories()) {
            Console.WriteLine(_logger.WriteLogEntry("Current Directory: " + directory.Name, "Current Directory: " + "[NAME REDACTED]"));

            string newDirectoryPath = Path.Combine(parentDirectory.FullName, directory.Name);
            string newDirectoryName = directory.Name;

            if (Directory.Exists(newDirectoryPath)) {
                Console.WriteLine(_logger.WriteLogEntry("Directory already exists..."));
                newDirectoryName = FindViableDirectoryName(directory, parentDirectory);
                newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);
            }

            Console.WriteLine(_logger.WriteLogEntry("Moving directory " + newDirectoryName + "...", "Moving directory " + "[NAME REDACTED]" + "..."));

            try {

                directory.MoveTo(newDirectoryPath);
            }
            catch (IOException e) {
                Console.WriteLine(_logger.WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(_logger.WriteLogEntry(e.Message, e.ToString()));
                _feedbackService.AddMoveFailure(new MoveFailure(directory, e));
            }
            catch (SecurityException e) {
                Console.WriteLine(_logger.WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(_logger.WriteLogEntry(e.Message, e.ToString()));
                _feedbackService.AddMoveFailure(new MoveFailure(directory, e));
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(_logger.WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(_logger.WriteLogEntry(e.Message, e.ToString()));
                _feedbackService.AddMoveFailure(new MoveFailure(directory, e));
            }
        }
    }


    private string FindViableFileName(FileInfo file, DirectoryInfo parentDirectory) {
        Console.WriteLine(_logger.WriteLogEntry("Finding new name..."));

        string extenstionlessFileName = file.Name[..file.Name.LastIndexOf(".")];
        string newFilePath = Path.Combine(parentDirectory.FullName, file.Name);
        int copyCounter = 0;
        string newFileName = "";

        while (File.Exists(newFilePath)) {
            copyCounter++;
            newFileName = extenstionlessFileName + "_copy" + copyCounter + file.Extension;
            newFilePath = Path.Combine(parentDirectory.FullName, newFileName);

            Console.WriteLine(_logger.WriteLogEntry("Checking name " + newFileName + "...", "Checking name " + "[NAME REDACTED]..."));
        }

        Console.WriteLine(_logger.WriteLogEntry("New Name: " + newFileName, "New Name: " + "[NAME REDACTED]"));

        return (newFileName);
    }


    private string FindViableDirectoryName(DirectoryInfo directory, DirectoryInfo parentDirectory) {
        Console.WriteLine(_logger.WriteLogEntry("Finding new name..."));

        string newDirectoryPath = Path.Combine(parentDirectory.FullName, directory.Name);
        int copyCounter = 1;
        string newDirectoryName = "";

        while (Directory.Exists(newDirectoryPath)) {
            copyCounter++;
            newDirectoryName = directory.Name + "_copy" + copyCounter;
            newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);

            Console.WriteLine(_logger.WriteLogEntry("Checking name " + newDirectoryName + "...", "Checing name [NAME REDACTED]..."));
        }
        Console.WriteLine(_logger.WriteLogEntry("New Name: " + newDirectoryName, "New Name: " + "[NAME REDACTED]"));

        return (newDirectoryName);
    }

}
