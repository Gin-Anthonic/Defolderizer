using System;
using System.IO;
//><
namespace Defolderizer {

    internal class Program {

        static void Main(string[] args) {

            if (args.Length == 0 || args.Length > 2) {
                Console.WriteLine("Invalid number of Arguments given.. exiting...");
                System.Environment.Exit(0);
            }

            if (!Directory.Exists( args[0])) {
                Console.WriteLine("Directory specified could not be found... exiting");
                System.Environment.Exit(0);
            }

            string[] validArgs = ["unfold", "defolderize", "recursive"];
            if (!validArgs.Contains(args[1])) {
                Console.WriteLine("Invalid argument for mode... exiting");
                System.Environment.Exit(0);
            }

            string currentWorkingDirectory = args[0];
            string mode = args[1];

            Console.WriteLine("Current Directory:" + currentWorkingDirectory + " Mode: " + mode);

            switch (mode) {
                case "unfold":
                    Unfold(currentWorkingDirectory);
                    break;
                case "defolderize":
                    Defolderize(currentWorkingDirectory);
                    break;
                case "recursive":
                    RecursiveDefolderize(currentWorkingDirectory);
                    break;
            }

        }


        public static void Defolderize(string currentDirectory) {
            foreach (string directory in Directory.GetDirectories(currentDirectory)) {
                Unfold(directory);
            }
        }


        public static void RecursiveDefolderize(string currentDirectory) {
            while (Directory.GetDirectories(currentDirectory).Length > 0) {
                Defolderize(currentDirectory);
            }
        }


        public static void Unfold(string currentDirectory) {
            string parentDirectory = Directory.GetParent(currentDirectory).FullName;
            MoveFiles(currentDirectory, parentDirectory);
            MoveDirectories(currentDirectory, parentDirectory);
            Directory.Delete(currentDirectory);
        }


        public static void MoveFiles(string currentDirectory, string parentDirectory) {
            string[] files = Directory.GetFiles(currentDirectory);
            foreach (string file in files) {
                Console.WriteLine("\nCurrent File: " + file.Substring(file.LastIndexOf("\\") + 1));
                string filePath = file;
                string fileName = GetExtensionlessFileName(filePath);
                string fileExtension = filePath.Substring(filePath.LastIndexOf("."));
                string newFilePath = parentDirectory + "\\" + fileName + fileExtension;
                if (File.Exists(newFilePath)) {
                    Console.WriteLine("File Already Exists...");
                    string newFileName = FindViableFileName(filePath, parentDirectory);
                    newFilePath = parentDirectory + "\\" + newFileName + fileExtension;
                }

                Console.WriteLine("Moving " + fileName + fileExtension + "...");
                File.Move(filePath, newFilePath);
            }
        }


        public static void MoveDirectories(string currentDirectory, string parentDirectory) {
            foreach (string directory in Directory.GetDirectories(currentDirectory)) {
                Console.WriteLine("\nCurrent Directory: " + directory.Substring(directory.LastIndexOf("\\") + 1));
                string directoryPath = directory;
                string directoryName = directory.Substring(directory.LastIndexOf("\\") + 1);
                string newDirectoryPath = parentDirectory + "\\" + directoryName;

                if (Directory.Exists(newDirectoryPath)) {
                    Console.WriteLine("Directory already exists...");
                    string newDirectoryName = FindViableDirectoryName(directoryPath, parentDirectory);
                    newDirectoryPath = parentDirectory + "\\" + newDirectoryName;
                }

                Console.WriteLine("Moving directory " + directoryName + "...");
                Directory.Move(directoryPath, newDirectoryPath);
            }
        }


        public static string FindViableFileName(string filePath, string parentDirectory) {
            Console.WriteLine("Finding new Filename...");
            string newFileName = GetExtensionlessFileName(filePath);
            string fileExtension = filePath.Substring(filePath.LastIndexOf("."));
            string newFilePath = parentDirectory + "\\" + newFileName + fileExtension;
            while (File.Exists(newFilePath)) {
                newFileName = newFileName + "_copy";
                newFilePath = parentDirectory + "\\" + newFileName + fileExtension;
            }
            Console.WriteLine("New Name: " + newFileName + fileExtension);
            return (newFileName);
        }


        public static string FindViableDirectoryName(string directoryPath, string parentDirectory) {
            Console.WriteLine("Finding new name...");
            string newDirectoryName = directoryPath.Substring(directoryPath.LastIndexOf("\\") + 1);
            string newDirectoryPath = parentDirectory + "\\" + newDirectoryName;
            while (Directory.Exists(newDirectoryPath)) {
                newDirectoryName = newDirectoryName + "_copy";
                newDirectoryPath = parentDirectory + "\\" + newDirectoryName;
            }
            Console.WriteLine("New Name: " + newDirectoryName);
            return (newDirectoryName);
        }


        public static string GetExtensionlessFileName(string filePath) {
            string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
            fileName = fileName.Substring(0, fileName.LastIndexOf("."));
            return (fileName);
        }

    }
}
