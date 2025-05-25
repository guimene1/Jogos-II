using UnityEngine;

public class FakePuzzleButton : Interactable
{
    public PasswordPuzzle puzzle;
    public AudioSource buttonAudio;

    public override void Interact()
    {
        Debug.Log("Botão falso pressionado! Reiniciando puzzle...");

        if (buttonAudio != null)
        {
            buttonAudio.Play();
        }

        if (puzzle != null)
        {
            puzzle.ResetPuzzleExternally();
        }
        base.Interact();
    }

    public override string GetInteractionMessage()
    {
        return "Pressionar botão";
    }
}