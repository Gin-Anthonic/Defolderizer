using Defolderizer.Services;
using Xunit;

namespace Defolderizer.Tests;

public class FileLoggerTests {

    [Fact]
    public void WriteLogEntryShouldCreateLog() {

        FileLoggingService loggingService = new FileLoggingService("");

        loggingService.WriteLogEntry("this is a user log","this is a developer log");

        FileInfo UserFile = new FileInfo("userLog.txt");
        FileInfo DeveloperFile = new FileInfo("developerLog.txt");

        bool success = (UserFile.Exists && DeveloperFile.Exists);

        Assert.True(success)

    }

}
