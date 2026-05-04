using System.Security;

namespace Defolderizer;

public class DefolderizerService {

    private readonly LoggingService logger;
    private readonly DirectoryInfo selectedDirectory;

    public DefolderizerService(DirectoryInfo givenDirectory, LoggingService logger) {
        this.logger = logger;
        selectedDirectory = givenDirectory;
    }



    public void Unfold() {
        this.Unfold(selectedDirectory);
    }


    public void Defolderize() {
        this.Defolderize(selectedDirectory);
    }


    public void RecursiveDefolderize() {
        Console.WriteLine(logger.WriteLogEntry("Recursively defolderizing directory " + selectedDirectory.FullName, "Recursively defolderizing directory [NAME REDACTED]"));

        DialogResult result = MessageBox.Show("You are about to recursively unfold the following directory: \n" + selectedDirectory.FullName + "\nProceed?", "Here be dragons!", MessageBoxButtons.YesNo);
        if (result == DialogResult.No) {
            Console.WriteLine(logger.WriteLogEntry("Process was aborted by user"));
            return;
        }
        RecusriveDefolderize(selectedDirectory);
    }



    private void Defolderize(DirectoryInfo currentDirectory) {
        Console.WriteLine(logger.WriteLogEntry("Defolderizing directory " + currentDirectory.FullName, "Defolderizing directory [NAME REDACTED]"));
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
        Console.WriteLine(logger.WriteLogEntry("Unfolding directory " + currentDirectory.FullName, "Unfolding directory [NAME REDACTED]"));

        if (currentDirectory.Parent == null) {
            Console.WriteLine(logger.WriteLogEntry("The directory " + currentDirectory.Name + " seems to have no parent, unfolding not possible... Exiting...", "The directory " + "[NAME REDACTED]" + " seems to have no parent, unfolding not possible... Exiting..."));
            return;
        }

        DirectoryInfo parentDirectory = currentDirectory.Parent;
        MoveFiles(currentDirectory, parentDirectory);
        MoveDirectories(currentDirectory, parentDirectory);

        if (Directory.EnumerateFileSystemEntries(currentDirectory.FullName).Count() != 0) {
            Console.WriteLine(logger.WriteLogEntry("Directory is not Empty... Removal Failed."));
            logger.AddToUserFeedback("\nThe Directory " + currentDirectory.Name + " was not removed as it still has contents!\n");
            return;
        }

        try {
            currentDirectory.Delete();
        }
        catch (IOException e) {
            Console.WriteLine(logger.WriteLogEntry("Removing the Directory  " + currentDirectory.Name + " failed because:", "Removing the Directory  " + "[NAME REDACTED]" + " failed because:"));
            Console.WriteLine(logger.WriteLogEntry(e.Message, e.ToString()));
            logger.AddToUserFeedback("\nAttempt to remove directory \"" + currentDirectory.Name + "\" failed due to the following Exception: \n" + e.Message + "\n");
        }
        catch (UnauthorizedAccessException e) {
            Console.WriteLine(logger.WriteLogEntry("Removing the Directory  " + currentDirectory.Name + " failed because:", "Removing the Directory  " + "[NAME REDACTED]" + " failed because:"));
            Console.WriteLine(logger.WriteLogEntry(e.Message, e.ToString()));
            logger.AddToUserFeedback("\nAttempt to remove directory \"" + currentDirectory.Name + "\" failed due to the following Exception: \n" + e.Message + "\n");
        }
    }


    private void MoveFiles(DirectoryInfo currentDirectory, DirectoryInfo parentDirectory) {
        FileInfo[] files = currentDirectory.GetFiles();
        foreach (FileInfo file in files) {
            Console.WriteLine(logger.WriteLogEntry("Current File: " + file.Name, "Current File: " + "[NAME REDACTED]"));

            string newFilePath = Path.Combine(parentDirectory.FullName, file.Name);
            string newFileName = file.Name;

            if (File.Exists(newFilePath)) {
                Console.WriteLine(logger.WriteLogEntry("File Already Exists..."));
                newFileName = FindViableFileName(file, parentDirectory);
                newFilePath = Path.Combine(parentDirectory.FullName, newFileName);
            }

            Console.WriteLine(logger.WriteLogEntry("Moving " + newFileName + "...", "Moving " + "[NAME REDACTED]" + "..."));

            try {
                file.MoveTo(newFilePath);
            }
            catch (IOException e) {
                Console.WriteLine(logger.WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(logger.WriteLogEntry(e.Message, e.ToString()));
                logger.AddMoveFailure(new MoveFailure(file, e));
            }
            catch (SecurityException e) {
                Console.WriteLine(logger.WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(logger.WriteLogEntry(e.Message, e.ToString()));
                logger.AddMoveFailure(new MoveFailure(file, e));
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(logger.WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(logger.WriteLogEntry(e.Message, e.ToString()));
                logger.AddMoveFailure(new MoveFailure(file, e));
            }
        }
    }


    private void MoveDirectories(DirectoryInfo currentDirectory, DirectoryInfo parentDirectory) {
        foreach (DirectoryInfo directory in currentDirectory.GetDirectories()) {
            Console.WriteLine(logger.WriteLogEntry("Current Directory: " + directory.Name, "Current Directory: " + "[NAME REDACTED]"));

            string newDirectoryPath = Path.Combine(parentDirectory.FullName, directory.Name);
            string newDirectoryName = directory.Name;

            if (Directory.Exists(newDirectoryPath)) {
                Console.WriteLine(logger.WriteLogEntry("Directory already exists..."));
                newDirectoryName = FindViableDirectoryName(directory, parentDirectory);
                newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);
            }

            Console.WriteLine(logger.WriteLogEntry("Moving directory " + newDirectoryName + "...", "Moving directory " + "[NAME REDACTED]" + "..."));

            try {

                directory.MoveTo(newDirectoryPath);
            }
            catch (IOException e) {
                Console.WriteLine(logger.WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(logger.WriteLogEntry(e.Message, e.ToString()));
                logger.AddMoveFailure(new MoveFailure(directory, e));
            }
            catch (SecurityException e) {
                Console.WriteLine(logger.WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(logger.WriteLogEntry(e.Message, e.ToString()));
                logger.AddMoveFailure(new MoveFailure(directory, e));
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(logger.WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(logger.WriteLogEntry(e.Message, e.ToString()));
                logger.AddMoveFailure(new MoveFailure(directory, e));
            }
        }
    }


    private string FindViableFileName(FileInfo file, DirectoryInfo parentDirectory) {
        Console.WriteLine(logger.WriteLogEntry("Finding new name..."));

        string extenstionlessFileName = file.Name[..file.Name.LastIndexOf(".")];
        string newFilePath = Path.Combine(parentDirectory.FullName, file.Name);
        int copyCounter = 0;
        string newFileName = "";

        while (File.Exists(newFilePath)) {
            copyCounter++;
            newFileName = extenstionlessFileName + "_copy" + copyCounter + file.Extension;
            newFilePath = Path.Combine(parentDirectory.FullName, newFileName);

            Console.WriteLine(logger.WriteLogEntry("Checking name " + newFileName + "...", "Checking name " + "[NAME REDACTED]..."));
        }

        Console.WriteLine(logger.WriteLogEntry("New Name: " + newFileName, "New Name: " + "[NAME REDACTED]"));

        return (newFileName);
    }


    private string FindViableDirectoryName(DirectoryInfo directory, DirectoryInfo parentDirectory) {
        Console.WriteLine(logger.WriteLogEntry("Finding new name..."));

        string newDirectoryPath = Path.Combine(parentDirectory.FullName, directory.Name);
        int copyCounter = 1;
        string newDirectoryName = "";

        while (Directory.Exists(newDirectoryPath)) {
            copyCounter++;
            newDirectoryName = directory.Name + "_copy" + copyCounter;
            newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);

            Console.WriteLine(logger.WriteLogEntry("Checking name " + newDirectoryName + "...", "Checing name [NAME REDACTED]..."));
        }
        Console.WriteLine(logger.WriteLogEntry("New Name: " + newDirectoryName, "New Name: " + "[NAME REDACTED]"));

        return (newDirectoryName);
    }

}
