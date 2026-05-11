using Defolderizer.Services;
using Defolderizer.Tests.Mocs;
using Defolderizer.Interfaces;
using Xunit;

namespace Defolderizer.Tests;

public class DefolderizerServiceTests {

    [Fact]
    public void FindViableFileNameShouldSucceed() {

        ILoggingService testLogger = new TestLoggingService();
        DefolderizerService defolderizerService = new DefolderizerService(testLogger, new FeedbackService());

        DirectoryInfo testDirectory = new DirectoryInfo("test");
        FileInfo testFile = new FileInfo("test.txt");

        testDirectory.Create();
        File.Create(Path.Combine(testDirectory.FullName,"test.txt"));
        File.Create(Path.Combine(testDirectory.FullName,"test_copy.txt"));
        File.Create(Path.Combine(testDirectory.FullName,"test_copy1.txt"));

        var expect = "test_copy2.txt";

        var actual = defolderizerService.FindViableFileName(testFile,testDirectory);

        Assert.Equal(expect,actual);

    }

}
