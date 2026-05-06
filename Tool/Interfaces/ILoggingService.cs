namespace Defolderizer.Interfaces;

public interface ILoggingService {

    public string WriteLogEntry(string userLogText, string developerLogText = "");

    public void Close();

}

