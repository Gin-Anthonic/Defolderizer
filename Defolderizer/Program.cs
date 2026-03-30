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
                Console.WriteLine("Invalid number of Arguments given(path,mode).. exiting...");
                System.Environment.Exit(0);
            }

            if (!Directory.Exists(args[0])) {
                Console.WriteLine("Directory specified could not be found... exiting");
                System.Environment.Exit(0);
            }

            string[] validArgs = ["unfold", "defolderize", "recursive", "test"];
            if (!validArgs.Contains(args[1])) {
                Console.WriteLine("Invalid argument for mode... exiting");
                System.Environment.Exit(0);
            }

            string currentDirectoryPath = args[0];
            string mode = args[1];

            DirectoryInfo currentDirectory = new DirectoryInfo(currentDirectoryPath);

            Console.WriteLine("Current Directory:" + currentDirectoryPath + " Mode: " + mode);

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
                    Console.WriteLine($"test {test.FullName} Exists: {test.Exists} ");
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
                Console.WriteLine($"The directory {currentDirectory.Name} seems to have no parent, unfolding not possible... Exiting...");
                return;
            }

            DirectoryInfo parentDirectory = currentDirectory.Parent;
            MoveFiles(currentDirectory, parentDirectory);
            MoveDirectories(currentDirectory, parentDirectory);
            if (Directory.EnumerateFileSystemEntries(currentDirectory.FullName).Count() != 0) {
                Console.WriteLine("Directory is not Empty... Removal Failed.");
                return;
            }
            try {

                currentDirectory.Delete();
            }
            catch (IOException e) {
                Console.WriteLine("Removing the Directory  " + currentDirectory.Name + " failed because:");
                Console.WriteLine(e.Message) ;
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine("Removing the Directory  " + currentDirectory.Name + " failed because:");
                Console.WriteLine(e.Message);
            }
        }


        public static void MoveFiles(DirectoryInfo currentDirectory, DirectoryInfo parentDirectory) {
            FileInfo[] files = currentDirectory.GetFiles();
            foreach (FileInfo file in files) {
                Console.WriteLine("\nCurrent File: " + file.Name);

                string newFilePath = Path.Combine(parentDirectory.FullName, file.Name);

                if (File.Exists(newFilePath)) {
                    Console.WriteLine("File Already Exists...");
                    string newFileName = FindViableFileName(file, parentDirectory);
                    newFilePath = Path.Combine(parentDirectory.FullName, newFileName) + file.Extension;
                }

                Console.WriteLine("Moving " + file.Name + "...");

                try {
                    file.MoveTo(newFilePath);
                }
                catch (IOException e) {
                    Console.WriteLine("Moving File " + file.Name + " failed because:");
                    Console.WriteLine(e.Message);
                }
                catch (SecurityException e) {
                    Console.WriteLine("Moving File " + file.Name + " failed because:");
                    Console.WriteLine(e.Message);
                }
                catch (UnauthorizedAccessException e) {
                    Console.WriteLine("Moving File " + file.Name + " failed because:");
                    Console.WriteLine(e.Message);
                }
            }
        }


        public static void MoveDirectories(DirectoryInfo currentDirectory, DirectoryInfo parentDirectory) {
            foreach (DirectoryInfo directory in currentDirectory.GetDirectories()) {
                Console.WriteLine("\nCurrent Directory: " + directory.Name);

                string newDirectoryPath = Path.Combine(parentDirectory.FullName, directory.Name);

                if (Directory.Exists(newDirectoryPath)) {
                    Console.WriteLine("Directory already exists...");
                    string newDirectoryName = FindViableDirectoryName(directory, parentDirectory);
                    newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);
                }

                Console.WriteLine("Moving directory " + directory.Name + "...");
                try {

                    directory.MoveTo(newDirectoryPath);
                }
                catch (IOException e) {
                    Console.WriteLine("Moving Directory "+ directory.Name + " failed because:");
                    Console.WriteLine(e.Message);
                }
                catch (SecurityException e) {
                    Console.WriteLine("Moving Directory " + directory.Name + " failed because:");
                    Console.WriteLine(e.Message);
                }
                catch (UnauthorizedAccessException e) {
                    Console.WriteLine("Moving Directory " + directory.Name + " failed because:");
                    Console.WriteLine(e.Message);
                }
            }
        }


        public static string FindViableFileName(FileInfo file, DirectoryInfo parentDirectory) {
            Console.WriteLine("Finding new Filename...");
            string newFileName = file.Name[..file.Name.LastIndexOf(".")];
            string newFilePath = Path.Combine(parentDirectory.FullName, newFileName) + file.Extension;
            while (File.Exists(newFilePath)) {
                newFileName = newFileName + "_copy";
                newFilePath = Path.Combine(parentDirectory.FullName, newFileName) + file.Extension;
            }
            Console.WriteLine("New Name: " + newFileName + file.Extension);
            return (newFileName);
        }


        public static string FindViableDirectoryName(DirectoryInfo directory, DirectoryInfo parentDirectory) {
            Console.WriteLine("Finding new name...");
            string newDirectoryName = directory.Name;
            string newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);
            while (Directory.Exists(newDirectoryPath)) {
                newDirectoryName = newDirectoryName + "_copy";
                newDirectoryPath = Path.Combine(parentDirectory.FullName, newDirectoryName);
            }
            Console.WriteLine("New Name: " + newDirectoryName);
            return (newDirectoryName);
        }



        public static void SetupTestFolder() {
            Directory.Delete("C:\\Users\\Work\\Desktop\\testing",true);
            Directory.CreateDirectory("C:\\Users\\Work\\Desktop\\testing");
            FileSystem.CopyDirectory("C:\\Users\\Work\\Documents\\gin", "C:\\Users\\Work\\Desktop\\testing\\gin");
        }
    }
}
