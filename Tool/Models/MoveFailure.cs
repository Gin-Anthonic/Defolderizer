namespace Defolderizer.Models;

public record MoveFailure(FileSystemInfo Entry, Exception CaughtException);