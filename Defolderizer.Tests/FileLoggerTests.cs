using Defolderizer.Services;
using Xunit;

namespace Defolderizer.Tests;

public class FileLoggerTests {

    [Fact]
    public void WriteLogEntryShouldCreateFiles() {
        FileLoggingService loggingService = new FileLoggingService("");
        loggingService.WriteLogEntry("this is a user log","this is a developer log");

        FileInfo UserFile = new FileInfo("userLog.txt");
        FileInfo DeveloperFile = new FileInfo("developerLog.txt");
        loggingService.Dispose();

        bool success = (UserFile.Exists && DeveloperFile.Exists);
        Assert.True(success);
        UserFile.Delete();
        DeveloperFile.Delete();
    }

    [Fact]
    public void WriteLogEntryShouldWriteLog() {
        FileLoggingService loggingService = new FileLoggingService("");
        loggingService.WriteLogEntry("this is a user log", "this is a developer log");

        FileInfo UserFile = new FileInfo("userLog.txt");
        FileInfo DeveloperFile = new FileInfo("developerLog.txt");
        loggingService.Dispose();

        bool success = (File.ReadAllText(UserFile.FullName).Contains("this is a user log") && File.ReadAllText(DeveloperFile.FullName).Contains("this is a developer log"));
        Assert.True(success);
        UserFile.Delete();
        DeveloperFile.Delete();
    }

}
