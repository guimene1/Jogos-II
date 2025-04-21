using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Jogar()
    {
        SceneManager.LoadScene("CenaJogo");
    }

    public void Sair()
    {
        Application.Quit();
    }
}