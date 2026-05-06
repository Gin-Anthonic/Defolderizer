namespace Defolderizer_Installer.Interfaces;

public interface ILoggingService{

    public void CreateInstallFailureLog(string outputContent, Exception exception, bool hadAdminPrivliges, bool? installForAllUsers);

}
