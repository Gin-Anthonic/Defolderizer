using System.Text.RegularExpressions;

namespace Defolderizer;

public class LoggingService {

    private string UserFeedback = "";
    private List<MoveFailure> MoveFailures = [];

    private readonly FileInfo userLogFile = new FileInfo("userLog.txt");
    private readonly FileInfo developerLogFile = new FileInfo("developerLog.txt");

    private readonly StreamWriter userWriter;
    private readonly StreamWriter developerWriter;

    private readonly Regex filePathFinderRegex = new Regex("'.*[\\\\/].*'");


    public LoggingService() {
        userWriter = userLogFile.AppendText();
        developerWriter = developerLogFile.AppendText();
    }


    public string WriteLogEntry(string userLogText, string developerLogText = "") {

        if (developerLogText == "") {
            developerLogText = userLogText;
        }
        userWriter.WriteLine(DateTime.Now + " - " + userLogText);
        developerWriter.WriteLine(DateTime.Now + " - " + filePathFinderRegex.Replace(developerLogText, "[FILEPATH REDACTED]"));
        return userLogText;
    }


    public void AddToUserFeedback(string messageContent) {
        UserFeedback += messageContent;
    }


    public void AddMoveFailure(MoveFailure moveFailure) {
        MoveFailures.Add(moveFailure);
    }


    public void ShowUserFeedbackPopup() {
        if (MoveFailures.Count > 0) {
            string message = "The following Files/Directories could not be moved: \n\n----------------------";

            foreach (MoveFailure failure in MoveFailures) {
                message += "\n\n" + failure.Entry.FullName + "\nWhat went wrong: \n" + failure.CaughtException.Message;
            }
            UserFeedback = message + "\n\n----------------------\n" + UserFeedback;
        }

        if (UserFeedback != "") {
            MessageBox.Show(UserFeedback);
        }
    }


    public void Close() {
        userWriter.Close();
        developerWriter.Close();
    }

}


public record MoveFailure(FileSystemInfo Entry, Exception CaughtException);