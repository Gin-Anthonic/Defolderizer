using Defolderizer.Interfaces;
using Defolderizer.Models;

namespace Defolderizer.Services;


public class FeedbackService : IFeedbackService {

    private string _userFeedback = "";
    private List<MoveFailure> _moveFailures = [];


    public void AddMessage(string messageContent) {
        _userFeedback += messageContent;
    }


    public void AddMoveFailure(MoveFailure moveFailure) {
        _moveFailures.Add(moveFailure);
    }


    public void ShowUserFeedback() {
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
}


