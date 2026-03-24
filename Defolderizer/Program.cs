using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;
//><
namespace Defolderizer {

    internal class Program {

        static void Main(string[] args) {

            SetupTestFolder();

            if (args.Length == 0 || args.Length > 2) {
                Console.WriteLine("Invalid number of Arguments given.. exiting...");
                System.Environment.Exit(0);
            }

            if (!Directory.Exists( args[0])) {
                Console.WriteLine("Directory specified could not be found... exiting");
                System.Environment.Exit(0);
            }

            string[] validArgs = ["unfold", "defolderize", "recursive","test"];
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
                    Console.WriteLine($"test {test.FullName} Exists: {test.Exists } " );
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

            DirectoryInfo parentDirectory = currentDirectory.Parent;
            MoveFiles(currentDirectory, parentDirectory);
            MoveDirectories(currentDirectory, parentDirectory);
            currentDirectory.Delete();
        }


        public static void MoveFiles(DirectoryInfo currentDirectory, DirectoryInfo parentDirectory) {
            FileInfo[] files = currentDirectory.GetFiles();
            foreach (FileInfo file in files) {
                Console.WriteLine("\nCurrent File: " + file.Name);

                string newFilePath = parentDirectory.FullName + "\\" + file.Name;

                if (File.Exists(newFilePath)) {
                    Console.WriteLine("File Already Exists...");
                    string newFileName = FindViableFileName(file, parentDirectory);
                    newFilePath = parentDirectory.FullName + "\\" + newFileName + file.Extension;
                }

                Console.WriteLine("Moving " + file.Name + "...");
                file.MoveTo(newFilePath);
            }
        }


        public static void MoveDirectories(DirectoryInfo currentDirectory, DirectoryInfo parentDirectory) {
            foreach (DirectoryInfo directory in currentDirectory.GetDirectories()) {
                Console.WriteLine("\nCurrent Directory: " + directory.Name);

                string newDirectoryPath = parentDirectory.FullName + "\\" + directory.Name;

                if (Directory.Exists(newDirectoryPath)) {
                    Console.WriteLine("Directory already exists...");
                    string newDirectoryName = FindViableDirectoryName(directory, parentDirectory);
                    newDirectoryPath = parentDirectory.FullName + "\\" + newDirectoryName;
                }

                Console.WriteLine("Moving directory " + directory.Name + "...");
                directory.MoveTo(newDirectoryPath);
            }
        }


        public static string FindViableFileName(FileInfo file, DirectoryInfo parentDirectory) {
            Console.WriteLine("Finding new Filename...");
            string newFileName = file.Name[..file.Name.LastIndexOf(".")];
            string newFilePath = parentDirectory.FullName + "\\" + newFileName + file.Extension;
            while (File.Exists(newFilePath)) {
                newFileName = newFileName + "_copy";
                newFilePath = parentDirectory.FullName + "\\" + newFileName + file.Extension;
            }
            Console.WriteLine("New Name: " + newFileName + file.Extension);
            return (newFileName);
        }


        public static string FindViableDirectoryName(DirectoryInfo directory, DirectoryInfo parentDirectory) {
            Console.WriteLine("Finding new name...");
            string newDirectoryName = directory.Name ;
            string newDirectoryPath = parentDirectory.FullName + "\\" + newDirectoryName;
            while (Directory.Exists(newDirectoryPath)) {
                newDirectoryName = newDirectoryName + "_copy";
                newDirectoryPath = parentDirectory.FullName + "\\" + newDirectoryName;
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
