using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
 void Start()
    {
        // Mostra e libera o cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void RestartGame()
    {
        SceneManager.LoadScene("Hospital"); 
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu"); 
    }

    public void QuitGame()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}