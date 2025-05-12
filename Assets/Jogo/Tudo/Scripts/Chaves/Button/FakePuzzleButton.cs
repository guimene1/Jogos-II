using UnityEngine;

public class FakePuzzleButton : Interactable
{
    public PasswordPuzzle puzzle;

    public override void Interact()
    {
        Debug.Log("Botão falso pressionado! Reiniciando puzzle...");
        if (puzzle != null)
        {
            puzzle.ResetPuzzleExternally();
        }
        base.Interact();
    }

    public override string GetInteractionMessage()
    {
        return "Pressionar botão (falso)";
    }
}
