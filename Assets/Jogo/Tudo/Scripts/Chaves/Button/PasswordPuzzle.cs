using System.Collections.Generic;
using UnityEngine;

public class PasswordPuzzle : MonoBehaviour
{
    [Header("Sequência correta dos botões (Índices ou IDs)")]
    public List<int> correctSequence = new List<int> { 1, 3, 2, 4 };

    [Header("Referência da porta a ser desbloqueada")]
    public Door doorToUnlock;

    [Header("Configurações de Áudio")]
    public AudioSource buttonPressAudio;
    public AudioSource correctSequenceAudio;
    public AudioSource wrongSequenceAudio;

    private List<int> playerSequence = new List<int>();

    public void PressButton(int buttonID)
    {
        playerSequence.Add(buttonID);
        Debug.Log("Botão pressionado: " + buttonID);

        // Tocar som do botão pressionado
        if (buttonPressAudio != null)
        {
            buttonPressAudio.Play();
        }

        if (playerSequence.Count == correctSequence.Count)
        {
            if (IsSequenceCorrect())
            {
                Debug.Log("Sequência correta! Porta destravada.");
                // Tocar som de acerto
                if (correctSequenceAudio != null)
                {
                    correctSequenceAudio.Play();
                }

                if (doorToUnlock != null)
                {
                    doorToUnlock.Unlock();
                }
            }
            else
            {
                Debug.Log("Sequência incorreta! Reiniciando puzzle.");
                // Tocar som de erro
                if (wrongSequenceAudio != null)
                {
                    wrongSequenceAudio.Play();
                }
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