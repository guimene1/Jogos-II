using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;  // Referência à luz da lanterna
    private bool isOn = false;  // Estado da lanterna (ligada ou desligada)

    void Start()
    {
        // Verifica se a lanterna foi atribuída
        if (flashlight != null)
        {
            flashlight.enabled = isOn;  // Define o estado inicial da lanterna como desligada
        }
    }

    void Update()
    {
        // Verifica se a tecla "F" foi pressionada
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Alterna o estado da lanterna
            isOn = !isOn;
            flashlight.enabled = isOn;
        }
    }
}
