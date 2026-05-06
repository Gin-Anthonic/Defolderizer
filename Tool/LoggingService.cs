using System.Text.RegularExpressions;

namespace Defolderizer;

public class LoggingService {

    private string _userFeedback = "";
    private List<MoveFailure> _moveFailures = [];

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


    public void AddToUserFeedback(string messageContent) {
        _userFeedback += messageContent;
    }


    public void AddMoveFailure(MoveFailure moveFailure) {
        _moveFailures.Add(moveFailure);
    }


    public void ShowUserFeedbackPopup() {
        if (_moveFailures.Count > 0) {
            string message = "The following Files/Directories could not be moved: \n\n----------------------";

            foreach (MoveFailure failure in _moveFailures) {
                message += "\n\n" + failure.Entry.FullName + "\nWhat went wrong: \n" + failure.CaughtException.Message;
            }
            _userFeedback = message + "\n\n----------------------\n" + _userFeedback;
        }

        if (_userFeedback != "") {
            MessageBox.Show(_userFeedback);
        }
    }


    public void Close() {
        _userWriter.Close();
        _developerWriter.Close();
    }

}


public record MoveFailure(FileSystemInfo Entry, Exception CaughtException);