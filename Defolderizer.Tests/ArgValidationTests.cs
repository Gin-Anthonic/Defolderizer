using Xunit;
using Defolderizer.Services;

namespace Defolderizer.Tests;

public class ArgValidationTests {

    [Theory]
    [InlineData("unfold")]
    [InlineData("defolderize")]
    [InlineData("recursive")]
    public void ArgValidationShouldSucceed(string mode) {
        ArgValidationService argValidationService = new ArgValidationService();
        ArgValidationService.ValidationResult expect = ArgValidationService.ValidationResult.Ok;

        ArgValidationService.ValidationResult actual = argValidationService.ValidateArgs(["C:\\",mode]);
       
        Assert.Equal(expect,actual);
    }


    [Theory]
    [InlineData("")]
    [InlineData("something else")]
    public void ArgValidationShouldFail(string mode) {
        ArgValidationService argValidationService = new ArgValidationService();
        ArgValidationService.ValidationResult expect = ArgValidationService.ValidationResult.InvalidMode;

        ArgValidationService.ValidationResult actual = argValidationService.ValidateArgs(["C:\\", mode]);

        Assert.Equal(expect, actual);
    }


    [Theory]
    [InlineData(new object[] { new string[] { "","","" } })]
    [InlineData(new object[] { new string[] { "" } })]
    [InlineData(new object[] { new string[] {  } })]
    public void ArgValidationWrongCountShouldFail(string[] args) {
        ArgValidationService argValidationService = new ArgValidationService();
        ArgValidationService.ValidationResult expect = ArgValidationService.ValidationResult.InvalidCount;

        ArgValidationService.ValidationResult actual = argValidationService.ValidateArgs(args);

        Assert.Equal(expect, actual);
    }

}


