using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToScene : MonoBehaviour
{
    [Header("Configurações de Teleporte")]
    [Tooltip("Arraste o objeto do jogador para este campo")]
    public GameObject player; // Variável para escolher o jogador manualmente
    
    [Tooltip("Nome exato da cena de destino (como está no Build Settings)")]
    public string sceneName;
    
    [Tooltip("Tempo de delay antes do teleporte")]
    public float teleportDelay = 0f;
    
    [Header("Configurações de Collider")]
    [Tooltip("Deve estar marcado para funcionar como trigger")]
    public bool isTrigger = true;

    private void Start()
    {
        // Configura automaticamente o collider como trigger
        if (TryGetComponent<Collider>(out var collider))
        {
            collider.isTrigger = isTrigger;
        }
        else
        {
            Debug.LogWarning("Adicione um Collider a este objeto para o teleporte funcionar!");
        }
        
        // Se o jogador não foi definido manualmente, tenta encontrar automaticamente
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("Nenhum jogador definido ou encontrado com a tag 'Player'!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no trigger é o jogador definido
        if (other.gameObject == player)
        {
            if (teleportDelay <= 0)
            {
                LoadScene();
            }
            else
            {
                Invoke(nameof(LoadScene), teleportDelay);
            }
        }
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            try
            {
                SceneManager.LoadScene(sceneName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao carregar cena: {e.Message}");
                Debug.Log("Verifique se:");
                Debug.Log("- O nome da cena está correto");
                Debug.Log("- A cena está adicionada no Build Settings");
            }
        }
        else
        {
            Debug.LogWarning("Nome da cena não definido!");
        }
    }
}