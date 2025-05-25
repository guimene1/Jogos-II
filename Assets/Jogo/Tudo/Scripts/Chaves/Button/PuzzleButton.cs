using UnityEngine;

public class PuzzleButton : Interactable
{
    public int buttonID;
    public PasswordPuzzle puzzle;
    public AudioSource buttonAudio;

    public override void Interact()
    {
        if (buttonAudio != null)
        {
            buttonAudio.Play();
        }
        puzzle.PressButton(buttonID);
        base.Interact();
    }

    public override string GetInteractionMessage()
    {
        return "Pressionar botão";
    }
}