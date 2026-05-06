using System.Text.RegularExpressions;

namespace Defolderizer;

public class LoggingService {


    private readonly FileInfo _userLogFile = new FileInfo("userLog.txt");
    private readonly FileInfo _developerLogFile = new FileInfo("developerLog.txt");

    private readonly StreamWriter _userWriter;
    private readonly StreamWriter _developerWriter;

    private readonly Regex _filePathFinderRegex = new Regex("'.*[\\\\/].*'");


    public LoggingService() {
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


    public void Close() {
        _userWriter.Close();
        _developerWriter.Close();
    }

}

