using UnityEngine;
using UnityEngine.SceneManagement;

public class FimManager : MonoBehaviour
{
    public void VoltarAoMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Quitar()
    {
        Application.Quit();
    }
}
