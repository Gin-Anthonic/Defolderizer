using System;
using System.IO;
using System.Transactions;
using Microsoft.VisualBasic;

internal class Program
{
    private static void Main(string[] args)
    {
        string currentWorkingDirectory = "C:\\Users\\Work\\Desktop\\testing\\gin";

        RecursiveDefolderize(currentWorkingDirectory);
    }


    public static void Defolderize(string currentDirectory)
    {
        foreach (string directory in Directory.GetDirectories(currentDirectory))
        {
           Unfold(directory); 
        }
    }


    public static void RecursiveDefolderize(string currentDirectory)
    {
        while (Directory.GetDirectories(currentDirectory).Length > 0)
        {
            Defolderize(currentDirectory);
        }
    }


    public static void Unfold(string currentDirectory)
    {
        string parentDirectory = Directory.GetParent(currentDirectory).FullName;
        MoveFiles(currentDirectory,parentDirectory);
        MoveDirectories(currentDirectory,parentDirectory);
        Directory.Delete(currentDirectory);
    }


    public static void MoveFiles(string currentDirectory, string parentDirectory)
    {
        string[] files = Directory.GetFiles(currentDirectory);
        foreach (string file in files){
            string filePath = file;
            string fileName = GetExtensionlessFileName(filePath);
            string fileExtension = filePath.Substring(filePath.LastIndexOf("."));
            string newFilePath = parentDirectory+"\\"+fileName+fileExtension;

            while (File.Exists(newFilePath))
            {   
                Console.WriteLine(fileName+" already exists in "+parentDirectory+"... Renaming...");
                File.Move(filePath, currentDirectory+"\\"+fileName+"_copy"+fileExtension);
                fileName = fileName+"_copy";
                filePath = currentDirectory+"\\"+fileName+fileExtension;
                newFilePath = parentDirectory+"\\"+fileName+fileExtension;
            }

            Console.WriteLine("Moving "+fileName);
            File.Move(filePath,newFilePath);
        }
    }

    public static void MoveDirectories(string currentDirectory, string parentDirectory)
    {
        foreach(string directory in Directory.GetDirectories(currentDirectory))
        {
            string directoryPath = directory;
            string directoryName = directory.Substring(directory.LastIndexOf("\\")+1);
            string newDirectoryPath = parentDirectory + "\\" + directoryName;

            while (Directory.Exists(newDirectoryPath))
            {
                Console.WriteLine(directoryName + " already exists in " + parentDirectory + "... Renaming...");
                Directory.Move(directoryPath, currentDirectory + "\\" + directoryName + "_copy");
                directoryName = directoryName + "_copy";
                directoryPath = currentDirectory + "\\" + directoryName;
                newDirectoryPath = parentDirectory + "\\" + directoryName;
            }

            Console.WriteLine("Moving "+directoryName+" to "+parentDirectory+"\\"+directoryName);
            Directory.Move(directoryPath,newDirectoryPath);
        }
    }

    public static string GetExtensionlessFileName(string filePath)
    {
        string fileName = filePath.Substring(filePath.LastIndexOf("\\")+1);
        fileName = fileName.Substring(0,fileName.LastIndexOf("."));
        return(fileName);
    }
  
}