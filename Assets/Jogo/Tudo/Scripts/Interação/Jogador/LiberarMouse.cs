using UnityEngine;

public class LiberarMouse : MonoBehaviour
{
    void Start()
    {
        // Libera o cursor do mouse
        Cursor.lockState = CursorLockMode.None;
        // Torna o cursor visível
        Cursor.visible = true;
    }
}