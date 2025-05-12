using System.Collections.Generic;
using UnityEngine;

public class PasswordPuzzle : MonoBehaviour
{
    [Header("Sequência correta dos botões (índices ou IDs)")]
    public List<int> correctSequence = new List<int> { 1, 3, 2, 4 };

    [Header("Referência da porta a ser desbloqueada")]
    public Door doorToUnlock;

    private List<int> playerSequence = new List<int>();

    public void PressButton(int buttonID)
    {
        playerSequence.Add(buttonID);
        Debug.Log("Botão pressionado: " + buttonID);

        if (playerSequence.Count == correctSequence.Count)
        {
            if (IsSequenceCorrect())
            {
                Debug.Log("Sequência correta! Porta destravada.");
                if (doorToUnlock != null)
                {
                    doorToUnlock.Unlock();
                }
            }
            else
            {
                Debug.Log("Sequência incorreta! Reiniciando puzzle.");
            }
            ResetPuzzle();
        }
    }

    private bool IsSequenceCorrect()
    {
        for (int i = 0; i < correctSequence.Count; i++)
        {
            if (playerSequence[i] != correctSequence[i])
                return false;
        }
        return true;
    }

    private void ResetPuzzle()
    {
        playerSequence.Clear();
    }

    public void ResetPuzzleExternally()
    {
        Debug.Log("Puzzle reiniciado por botão falso.");
        playerSequence.Clear();
    }

}
