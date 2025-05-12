using UnityEngine;

public class ResetPuzzleButton : Interactable
{
    public PasswordPuzzle puzzle;

    public override void Interact()
    {
        if (puzzle != null)
        {
            puzzle.ResetPuzzleExternally();
            Debug.Log("Puzzle resetado manualmente.");
        }
        base.Interact();
    }

    public override string GetInteractionMessage()
    {
        return "Resetar puzzle";
    }
}
