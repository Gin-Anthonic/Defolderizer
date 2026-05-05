

using System.IO;

namespace Defolderizer_Installer;

public class LoggingService {
    
    public void CreateInstallFailureLog(string outputContent, Exception exception, bool hadAdminPrivliges, bool? installForAllUsers) {
        string logContent = $"Admin Status: {hadAdminPrivliges.ToString()}\n" +
                            $"Install for all Users: {installForAllUsers.ToString()}\n\n" +
                            $"Install Output: \n{outputContent}\n\n" +
                            $"Exception: {exception.ToString()}";
        string fileName = createLogFileName();
        FileInfo logFile = new FileInfo(fileName);
        File.WriteAllText(logFile.FullName, logContent);
    }


    private string createLogFileName() {
        var now = DateTime.Now;
        string fileName = $"{now.Year}-{now.Month}-{now.Day}-{now.Hour}-{now.Minute}.log";

        return fileName;
    }

}

