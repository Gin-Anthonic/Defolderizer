using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows.Forms;
//><
namespace Defolderizer {

    internal class Program {

        public static string UserMessage { get; set; } = "";
        public static MoveFailure[] MoveFailures = [];

        static void Main(string[] args) {

            //SetupTestFolder();

            if (args.Length != 2) {
                Console.WriteLine(WriteLogEntry("Invalid number of Arguments given(path,mode).. exiting..."));
                System.Environment.Exit(0);
            }

            if (!Directory.Exists(args[0])) {
                Console.WriteLine(WriteLogEntry("Directory specified could not be found... exiting"));
                System.Environment.Exit(0);
            }

            string[] validArgs = ["unfold", "defolderize", "recursive", "test"];
            if (!validArgs.Contains(args[1])) {
                Console.WriteLine(WriteLogEntry("Invalid argument for mode... exiting"));
                System.Environment.Exit(0);
            }

            string currentDirectoryPath = args[0];
            string mode = args[1];

            DirectoryInfo currentDirectory = new DirectoryInfo(currentDirectoryPath);

            Console.WriteLine(WriteLogEntry("Program Started..."));
            Console.WriteLine(WriteLogEntry("Current Directory: " + currentDirectoryPath + " Mode: " + mode, "Current Directory: [NAME REDACTED] Mode: " + mode));

            switch (mode) {
                case "unfold":
                    Unfold(currentDirectory);
                    break;
                case "defolderize":
                    Defolderize(currentDirectory);
                    break;
                case "recursive":
                    RecursiveDefolderize(currentDirectory);
                    break;
                case "test":
                    FileInfo test = new FileInfo(currentDirectoryPath + "\\nonExistantFile.txt");
                    Console.WriteLine(WriteLogEntry($"test {test.FullName} Exists: {test.Exists} "));
                    break;
            }
            if (MoveFailures.Length > 0) {
                string message = "The following Files/Directories could not be moved: \n\n----------------------";
            
                foreach(MoveFailure failure in MoveFailures) {
                    message += "\n\n" + failure.Entry.FullName + "\nWhat went wrong: \n" + failure.CatchedException.Message;
                }
                UserMessage = message + "\n\n----------------------\n" + UserMessage;
            }

            if (UserMessage != "") {
                MessageBox.Show(UserMessage);
            }
        }


        public static void Defolderize(DirectoryInfo currentDirectory) {
            Console.WriteLine(WriteLogEntry("Defolderizing directory " + currentDirectory.FullName,"Defolderizing directory [NAME REDACTED]"));
            foreach (DirectoryInfo directory in currentDirectory.GetDirectories()) {
                Unfold(directory);
            }
        }


        public static void RecursiveDefolderize(DirectoryInfo currentDirectory) {
            Console.WriteLine(WriteLogEntry("Recursively defolderizing directory " + currentDirectory.FullName, "Recursively defolderizing directory [NAME REDACTED]"));
            while (currentDirectory.GetDirectories().Length > 0) {
                Defolderize(currentDirectory);
            }
        }


        public static void Unfold(DirectoryInfo currentDirectory) {
            Console.WriteLine(WriteLogEntry("Unfolding directory " + currentDirectory.FullName, "Unfolding directory [NAME REDACTED]"));
            if (currentDirectory.Parent == null) {
                Console.WriteLine(WriteLogEntry("The directory " + currentDirectory.Name + " seems to have no parent, unfolding not possible... Exiting...", "The directory " + "[NAME REDACTED]" + " seems to have no parent, unfolding not possible... Exiting..."));
                return;
            }

            DirectoryInfo parentDirectory = currentDirectory.Parent;
            MoveFiles(currentDirectory, parentDirectory);
            MoveDirectories(currentDirectory, parentDirectory);
            if (Directory.EnumerateFileSystemEntries(currentDirectory.FullName).Count() != 0) {
                Console.WriteLine(WriteLogEntry("Directory is not Empty... Removal Failed."));
                UserMessage += "\nThe Directory " + currentDirectory.Name + " was not removed as it still has contents!\n";
                return;
            }
            try {

                currentDirectory.Delete();
            }
            catch (IOException e) {
                Console.WriteLine(WriteLogEntry("Removing the Directory  " + currentDirectory.Name + " failed because:", "Removing the Directory  " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(WriteLogEntry(e.Message,e.ToString()));
                UserMessage += "\nAttempt to remove directory \"" + currentDirectory.Name + "\" failed due to the following Exception: \n" + e.Message + "\n";
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(WriteLogEntry("Removing the Directory  " + currentDirectory.Name + " failed because:", "Removing the Directory  " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                UserMessage += "\nAttempt to remove directory \"" + currentDirectory.Name + "\" failed due to the following Exception: \n" + e.Message + "\n";
            }
        }


        public static void MoveFiles(DirectoryInfo currentDirectory, DirectoryInfo parentDirectory) {
            FileInfo[] files = currentDirectory.GetFiles();
            foreach (FileInfo file in files) {
                Console.WriteLine(WriteLogEntry("Current File: " + file.Name, "Current File: " + "[NAME REDACTED]"));

                string newFilePath = Path.Combine(parentDirectory.FullName, file.Name);
                string newFileName = file.Name;

                if (File.Exists(newFilePath)) {
                    Console.WriteLine(WriteLogEntry("File Already Exists..."));
                    newFileName = FindViableFileName(file, parentDirectory);
                    newFilePath = Path.Combine(parentDirectory.FullName, newFileName);
                }

                Console.WriteLine(WriteLogEntry("Moving " + newFileName + "...", "Moving " + "[NAME REDACTED]" + "..."));

                try {
                    file.MoveTo(newFilePath);
                }
                catch (IOException e) {
                    Console.WriteLine(WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                    Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                    MoveFailures = MoveFailures.Append(new MoveFailure(file, e)).ToArray();
                }
                catch (SecurityException e) {
                    Console.WriteLine(WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                    Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                    MoveFailures = MoveFailures.Append(new MoveFailure(file, e)).ToArray();
                }
                catch (UnauthorizedAccessException e) {
                    Console.WriteLine(WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                    Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                    MoveFailures = MoveFailures.Append(new MoveFailure(file, e)).ToArray();
                }
            }
        }


        public static void MoveDirectories(DirectoryInfo currentDirectory, DirectoryInfo parentDirectory) {
            foreach (DirectoryInfo directory in currentDirectory.GetDirectories()) {
                Console.WriteLine(WriteLogEntry("Current Directory: " + directory.Name, "Current Directory: " + "[NAME REDACTED]"));

                string newDirectoryPath = Path.Combine(parentDirectory.FullName, directory.Name);
                string newDirectoryName = directory.Name;

                if (Directory.Exists(newDirectoryPath)) {
                    Console.WriteLine(WriteLogEntry("Directory already exists..."));
                    newDirectoryName = FindViableDirectoryName(directory, parentDirectory);
                    newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);
                }

                Console.WriteLine(WriteLogEntry("Moving directory " + newDirectoryName + "...", "Moving directory " + "[NAME REDACTED]" + "..."));

                try {

                    directory.MoveTo(newDirectoryPath);
                }
                catch (IOException e) {
                    Console.WriteLine(WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                    Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                    MoveFailures = MoveFailures.Append(new MoveFailure(directory,e)).ToArray();
                }
                catch (SecurityException e) {
                    Console.WriteLine(WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                    Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                    MoveFailures = MoveFailures.Append(new MoveFailure(directory, e)).ToArray();
                }
                catch (UnauthorizedAccessException e) {
                    Console.WriteLine(WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                    Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                    MoveFailures = MoveFailures.Append(new MoveFailure(directory, e)).ToArray();
                }
            }
        }


        public static string FindViableFileName(FileInfo file, DirectoryInfo parentDirectory) {
            Console.WriteLine(WriteLogEntry("Finding new name..."));

            string extenstionlessFileName = file.Name[..file.Name.LastIndexOf(".")];
            string newFilePath = Path.Combine(parentDirectory.FullName, file.Name);
            int copyCounter = 0;
            string newFileName = "";
           
            while (File.Exists(newFilePath)) {
                copyCounter ++;
                newFileName = extenstionlessFileName + "_copy" + copyCounter + file.Extension;
                newFilePath = Path.Combine(parentDirectory.FullName, newFileName);
                
                Console.WriteLine(WriteLogEntry("Checking name " + newFileName + "...", "Checking name " + "[NAME REDACTED]..."));
            }

            Console.WriteLine(WriteLogEntry("New Name: " + newFileName , "New Name: " + "[NAME REDACTED]"));

            return (newFileName);
        }


        public static string FindViableDirectoryName(DirectoryInfo directory, DirectoryInfo parentDirectory) {
            Console.WriteLine(WriteLogEntry("Finding new name..."));

            string newDirectoryPath = Path.Combine(parentDirectory.FullName, directory.Name);
            int copyCounter = 1;
            string newDirectoryName = "";

            while (Directory.Exists(newDirectoryPath)) {
                copyCounter++;
                newDirectoryName = directory.Name + "_copy" + copyCounter;
                newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);

                Console.WriteLine(WriteLogEntry("Checking name " + newDirectoryName + "...", "Checing name [NAME REDACTED]..."));
            }
            Console.WriteLine(WriteLogEntry("New Name: " + newDirectoryName, "New Name: " + "[NAME REDACTED]"));

            return (newDirectoryName);
        }



        public static string WriteLogEntry(string userLogText, string developerLogText = "") {

            Regex pathFinderRegex = new Regex("'.*[\\\\/].*'");

            if (developerLogText == "") {
                developerLogText = userLogText;
            }
            FileInfo userLogFile = new FileInfo("userLog.txt");
            FileInfo developerLogFile = new FileInfo("developerLog.txt");
            StreamWriter userWriter = userLogFile.AppendText();
            userWriter.WriteLine(DateTime.Now + " - " + userLogText);
            userWriter.Close();
            StreamWriter developerWriter = developerLogFile.AppendText();
            developerWriter.WriteLine(DateTime.Now + " - " + pathFinderRegex.Replace(developerLogText,"[FILEPATH REDACTED]"));
            developerWriter.Close();
            return userLogText;
        }


        public static void SetupTestFolder() {
            Directory.Delete("C:\\Users\\Work\\Desktop\\testing",true);
            Directory.CreateDirectory("C:\\Users\\Work\\Desktop\\testing");
            FileSystem.CopyDirectory("C:\\Users\\Work\\Documents\\gin", "C:\\Users\\Work\\Desktop\\testing\\gin");
        }
    }

    public struct MoveFailure {
        public FileSystemInfo Entry {  get; set; }
        public Exception CatchedException { get; set; }

        public MoveFailure(FileSystemInfo entry, Exception exception) {
            this.Entry = entry;
            this.CatchedException = exception;
        }
    }
}
