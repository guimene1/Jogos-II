using UnityEngine;

public class DestroyParentWhenChildrenGone : MonoBehaviour
{
    [Tooltip("Número de filhos que precisam ser destruídos antes deste objeto ser destruído")]
    public int requiredChildrenToDestroy = 4;

    private int destroyedChildrenCount = 0;

    void Start()
    {
        // Verifica se o objeto tem filhos suficientes
        if (transform.childCount < requiredChildrenToDestroy)
        {
            Debug.LogWarning("O objeto pai tem menos filhos do que o número necessário para destruí-lo!");
        }

        // Registra o método para ser chamado quando qualquer filho for destruído
        foreach (Transform child in transform)
        {
            // Adiciona um componente auxiliar a cada filho para detectar quando for destruído
            ChildDestroyNotifier notifier = child.gameObject.AddComponent<ChildDestroyNotifier>();
            notifier.OnChildDestroyed += HandleChildDestroyed;
        }
    }

    private void HandleChildDestroyed()
    {
        destroyedChildrenCount++;
        
        if (destroyedChildrenCount >= requiredChildrenToDestroy)
        {
            Destroy(gameObject);
        }
    }

    // Classe auxiliar para detectar quando um filho é destruído
    private class ChildDestroyNotifier : MonoBehaviour
    {
        public delegate void ChildDestroyed();
        public event ChildDestroyed OnChildDestroyed;

        private void OnDestroy()
        {
            if (OnChildDestroyed != null)
            OnChildDestroyed();
        }
    }
}