using Defolderizer.Interfaces;
using System.Text.RegularExpressions;

namespace Defolderizer.Services;

public class FileLoggingService : ILoggingService, IDisposable {


    private readonly FileInfo _userLogFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "userLog.txt"));
    private readonly FileInfo _developerLogFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "developerLog.txt"));

    private readonly StreamWriter _userWriter;
    private readonly StreamWriter _developerWriter;

    private readonly Regex _filePathFinderRegex = new Regex("'.*[\\\\/].*'");


    public FileLoggingService() {
        _userWriter = _userLogFile.AppendText();
        _developerWriter = _developerLogFile.AppendText();
    }


    public string WriteLogEntry(string userLogText, string developerLogText = "") {

        if (developerLogText == "") {
            developerLogText = userLogText;
        }
        _userWriter.WriteLine(DateTime.Now + " - " + userLogText);
        _developerWriter.WriteLine(DateTime.Now + " - " + _filePathFinderRegex.Replace(developerLogText, "[FILEPATH REDACTED]"));
        return userLogText;
    }


    public void Dispose() {
        _userWriter.Close();
        _userWriter.Dispose();
        _developerWriter.Close();
        _developerWriter.Dispose();
    }
}

