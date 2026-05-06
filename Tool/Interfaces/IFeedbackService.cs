using Defolderizer.Models;

namespace Defolderizer.Interfaces;

public interface IFeedbackService {

    public void AddMessage(string messageContent);

    public void AddMoveFailure(MoveFailure moveFailure);

    public void ShowUserFeedback();

}
