using UnityEngine;

public class PuzzleButton : Interactable
{
    public int buttonID;
    public PasswordPuzzle puzzle;

    public override void Interact()
    {
        puzzle.PressButton(buttonID);
        base.Interact();
    }

    public override string GetInteractionMessage()
    {
        return "Pressionar botão";
    }
}
