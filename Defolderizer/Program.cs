using System;
using System.IO;
using Microsoft.VisualBasic;
internal class Program
{
    private static void Main(string[] args)
    {
        string currentWorkingDirectory = "C:\\Users\\Work\\Desktop\\testing\\gin";

        Unfold(currentWorkingDirectory);
    }

    public static void Unfold(string currentDirectory)
    {
        string parentDirectory = Directory.GetParent(currentDirectory).FullName;
       
        string[] files = Directory.GetFiles(currentDirectory);
        foreach (string file in files){
            string fileName = file.Substring(file.LastIndexOf("\\")+1);
            fileName = fileName.Substring(0,fileName.LastIndexOf("."));
            string fileExtension = file.Substring(file.LastIndexOf("."));
            Console.WriteLine(fileName);
            string filePath = currentDirectory+"\\"+fileName+fileExtension;
            string newFilePath = parentDirectory+"\\"+fileName+fileExtension;

            while (File.Exists(newFilePath))
            {
                Console.WriteLine(fileName+" already exists in "+parentDirectory+"... Renaming...");
                FileSystem.Rename(filePath,currentDirectory+"\\"+fileName+"_copy"+fileExtension);
                fileName = fileName+"_copy";
                filePath = currentDirectory+"\\"+fileName+fileExtension;
                newFilePath = parentDirectory+"\\"+fileName+fileExtension;
            }

            Console.WriteLine("Moving "+fileName);
            File.Move(filePath,newFilePath);
        }

        foreach(string directory in Directory.GetDirectories(currentDirectory))
        {
            string directoryName = directory.Substring(directory.LastIndexOf("\\")+1);
            Console.WriteLine("Moving "+directoryName+" to "+parentDirectory+"\\"+directoryName);
            Directory.Move(directory,parentDirectory+"\\"+directoryName);
        }

        Directory.Delete(currentDirectory);
    }
}