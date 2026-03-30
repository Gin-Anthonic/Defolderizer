using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Security;
//><
namespace Defolderizer {

    internal class Program {

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
        }


        public static void Defolderize(DirectoryInfo currentDirectory) {
            foreach (DirectoryInfo directory in currentDirectory.GetDirectories()) {
                Unfold(directory);
            }
        }


        public static void RecursiveDefolderize(DirectoryInfo currentDirectory) {
            while (currentDirectory.GetDirectories().Length > 0) {
                Defolderize(currentDirectory);
            }
        }


        public static void Unfold(DirectoryInfo currentDirectory) {
            if (currentDirectory.Parent == null) {
                Console.WriteLine(WriteLogEntry("The directory " + currentDirectory.Name + " seems to have no parent, unfolding not possible... Exiting...", "The directory " + "[NAME REDACTED]" + " seems to have no parent, unfolding not possible... Exiting..."));
                return;
            }

            DirectoryInfo parentDirectory = currentDirectory.Parent;
            MoveFiles(currentDirectory, parentDirectory);
            MoveDirectories(currentDirectory, parentDirectory);
            if (Directory.EnumerateFileSystemEntries(currentDirectory.FullName).Count() != 0) {
                Console.WriteLine(WriteLogEntry("Directory is not Empty... Removal Failed."));
                return;
            }
            try {

                currentDirectory.Delete();
            }
            catch (IOException e) {
                Console.WriteLine(WriteLogEntry("Removing the Directory  " + currentDirectory.Name + " failed because:", "Removing the Directory  " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(WriteLogEntry(e.Message,e.ToString()));
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(WriteLogEntry("Removing the Directory  " + currentDirectory.Name + " failed because:", "Removing the Directory  " + "[NAME REDACTED]" + " failed because:"));
                Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
            }
        }


        public static void MoveFiles(DirectoryInfo currentDirectory, DirectoryInfo parentDirectory) {
            FileInfo[] files = currentDirectory.GetFiles();
            foreach (FileInfo file in files) {
                Console.WriteLine(WriteLogEntry("Current File: " + file.Name, "Current File: " + "[NAME REDACTED]"));

                string newFilePath = Path.Combine(parentDirectory.FullName, file.Name);

                if (File.Exists(newFilePath)) {
                    Console.WriteLine(WriteLogEntry("File Already Exists..."));
                    string newFileName = FindViableFileName(file, parentDirectory);
                    newFilePath = Path.Combine(parentDirectory.FullName, newFileName) + file.Extension;
                }

                Console.WriteLine(WriteLogEntry("Moving " + file.Name + "...", "Moving " + "[NAME REDACTED]" + "..."));

                try {
                    file.MoveTo(newFilePath);
                }
                catch (IOException e) {
                    Console.WriteLine(WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                    Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                }
                catch (SecurityException e) {
                    Console.WriteLine(WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                    Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                }
                catch (UnauthorizedAccessException e) {
                    Console.WriteLine(WriteLogEntry("Moving File " + file.Name + " failed because:", "Moving File " + "[NAME REDACTED]" + " failed because:"));
                    Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                }
            }
        }


        public static void MoveDirectories(DirectoryInfo currentDirectory, DirectoryInfo parentDirectory) {
            foreach (DirectoryInfo directory in currentDirectory.GetDirectories()) {
                Console.WriteLine(WriteLogEntry("Current Directory: " + directory.Name, "Current Directory: " + "[NAME REDACTED]"));

                string newDirectoryPath = Path.Combine(parentDirectory.FullName, directory.Name);

                if (Directory.Exists(newDirectoryPath)) {
                    Console.WriteLine(WriteLogEntry("Directory already exists..."));
                    string newDirectoryName = FindViableDirectoryName(directory, parentDirectory);
                    newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);
                }

                Console.WriteLine(WriteLogEntry("Moving directory " + directory.Name + "...", "Moving directory " + "[NAME REDACTED]" + "..."));
                try {

                    directory.MoveTo(newDirectoryPath);
                }
                catch (IOException e) {
                    Console.WriteLine(WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                    Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                }
                catch (SecurityException e) {
                    Console.WriteLine(WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                    Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                }
                catch (UnauthorizedAccessException e) {
                    Console.WriteLine(WriteLogEntry("Moving Directory " + directory.Name + " failed because:", "Moving Directory " + "[NAME REDACTED]" + " failed because:"));
                    Console.WriteLine(WriteLogEntry(e.Message, e.ToString()));
                }
            }
        }


        public static string FindViableFileName(FileInfo file, DirectoryInfo parentDirectory) {
            Console.WriteLine(WriteLogEntry("Finding new Filename..."));
            string newFileName = file.Name[..file.Name.LastIndexOf(".")];
            string newFilePath = Path.Combine(parentDirectory.FullName, newFileName) + file.Extension;
            while (File.Exists(newFilePath)) {
                newFileName = newFileName + "_copy";
                newFilePath = Path.Combine(parentDirectory.FullName, newFileName) + file.Extension;
            }
            Console.WriteLine(WriteLogEntry("New Name: " + newFileName + file.Extension, "New Name: " + "[NAME REDACTED]"));
            return (newFileName);
        }


        public static string FindViableDirectoryName(DirectoryInfo directory, DirectoryInfo parentDirectory) {
            Console.WriteLine(WriteLogEntry("Finding new name..."));
            string newDirectoryName = directory.Name;
            string newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);
            while (Directory.Exists(newDirectoryPath)) {
                newDirectoryName = newDirectoryName + "_copy";
                newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);
            }
            Console.WriteLine(WriteLogEntry("New Name: " + newDirectoryName, "New Name: " + "[NAME REDACTED]"));
            return (newDirectoryName);
        }



        public static string WriteLogEntry(string userLogText, string developerLogText = "") {

            if (developerLogText == "") {
                developerLogText = userLogText;
            }
            FileInfo userLogFile = new FileInfo("userLog.txt");
            FileInfo developerLogFile = new FileInfo("developerLog.txt");
            StreamWriter userWriter = userLogFile.AppendText();
            userWriter.WriteLine(DateTime.Now + " - " + userLogText);
            userWriter.Close();
            StreamWriter developerWriter = developerLogFile.AppendText();
            developerWriter.WriteLine(DateTime.Now + " - " + developerLogText);
            developerWriter.Close();
            return userLogText;
        }


        public static void SetupTestFolder() {
            Directory.Delete("C:\\Users\\Work\\Desktop\\testing",true);
            Directory.CreateDirectory("C:\\Users\\Work\\Desktop\\testing");
            FileSystem.CopyDirectory("C:\\Users\\Work\\Documents\\gin", "C:\\Users\\Work\\Desktop\\testing\\gin");
        }
    }
}
