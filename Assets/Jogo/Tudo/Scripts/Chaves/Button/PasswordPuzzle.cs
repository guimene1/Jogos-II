using System.Collections.Generic;
using UnityEngine;

public class PasswordPuzzle : MonoBehaviour
{
    [Header("Sequ�ncia correta dos bot�es (�ndices ou IDs)")]
    public List<int> correctSequence = new List<int> { 1, 3, 2, 4 };

    [Header("Refer�ncia da porta a ser desbloqueada")]
    public Door doorToUnlock;

    [Header("Configura��es de �udio")]
    public AudioSource buttonPressAudio;
    public AudioSource correctSequenceAudio;
    public AudioSource wrongSequenceAudio;

    private List<int> playerSequence = new List<int>();

    public void PressButton(int buttonID)
    {
        playerSequence.Add(buttonID);
        Debug.Log("Bot�o pressionado: " + buttonID);

        // Tocar som do bot�o pressionado
        if (buttonPressAudio != null)
        {
            buttonPressAudio.Play();
        }

        if (playerSequence.Count == correctSequence.Count)
        {
            if (IsSequenceCorrect())
            {
                Debug.Log("Sequ�ncia correta! Porta destravada.");
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
                Debug.Log("Sequ�ncia incorreta! Reiniciando puzzle.");
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
        Debug.Log("Puzzle reiniciado por bot�o falso.");
        playerSequence.Clear();
    }
}