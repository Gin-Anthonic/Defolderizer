using Defolderizer.Interfaces;
using System.Text.RegularExpressions;

namespace Defolderizer.Services;

public class FileLoggingService : ILoggingService, IDisposable {

    private readonly string _logFilePath;
    private readonly FileInfo _userLogFile;
    private readonly FileInfo _developerLogFile;
    private readonly StreamWriter _userWriter = StreamWriter.Null;
    private readonly StreamWriter _developerWriter = StreamWriter.Null;

    private readonly Regex _filePathFinderRegex = new Regex("'.*[\\\\/].*'");


    public FileLoggingService() {
        _logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "programs", "Defolderizer");
        _userLogFile = new FileInfo(Path.Combine(_logFilePath, "userLog.txt"));
        _developerLogFile = new FileInfo(Path.Combine(_logFilePath, "developerLog.txt"));
        try {
            _userWriter = _userLogFile.AppendText();
            _developerWriter = _developerLogFile.AppendText();

        }
        catch (Exception e) {
            MessageBox.Show("Logfiles unaccessable. No logs will be created.\nError:\n" + e.Message);
        }
    }


    public string WriteLogEntry(string userLogText, string developerLogText = "") {

        if (developerLogText == "") {
            developerLogText = userLogText;
        }

        try {
            _userWriter.WriteLine(DateTime.Now + " - " + userLogText);
            _developerWriter.WriteLine(DateTime.Now + " - " + _filePathFinderRegex.Replace(developerLogText, "[FILEPATH REDACTED]"));
        }
        catch { }
        return userLogText;
    }


    public void Dispose() {
        _userWriter.Close();
        _userWriter.Dispose();
        _developerWriter.Close();
        _developerWriter.Dispose();
    }
}

