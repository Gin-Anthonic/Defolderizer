using Defolderizer.Interfaces;

namespace Defolderizer.Services;

public class ArgValidationService {

    string[] validArgs = ["unfold", "defolderize", "recursive"];
    
    public enum ValidationResult { Ok, InvalidCount, InvalidDirectory, InvalidMode }

    public Dictionary<ValidationResult, String> ResultMessages = new Dictionary<ValidationResult, String>() {
        {ValidationResult.InvalidCount,     "Invalid number of Arguments given(path,mode).. exiting..." },
        {ValidationResult.InvalidDirectory, "Directory specified could not be found or is faulty... exiting..." },
        {ValidationResult.InvalidMode,     "Invalid argument for mode... exiting" }
    };
    
    public ValidationResult ValidateArgs(string[] args) {
        if (args.Length != 2) {
            return ValidationResult.InvalidCount;
        }
        if (!Directory.Exists(args[0])) {
            return ValidationResult.InvalidDirectory;
        }
        if (!validArgs.Contains(args[1])) {
            return ValidationResult.InvalidMode;
        }
        return ValidationResult.Ok;
    }

}
