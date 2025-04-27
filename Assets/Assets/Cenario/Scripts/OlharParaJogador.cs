using UnityEngine;

public class OlharParaJogador : MonoBehaviour
{
    public Transform player;
    public float distanciaMaxima = 10f; // distância máxima para olhar

    void Update()
    {
        if (player != null)
        {
            float distancia = Vector3.Distance(transform.position, player.position);

            if (distancia <= distanciaMaxima)
            {
                Vector3 direcao = (player.position - transform.position).normalized;
                direcao.y = 0; // mantém o objeto reto, sem olhar para cima ou para baixo

                if (direcao != Vector3.zero)
                {
                    Quaternion rotacaoDesejada = Quaternion.LookRotation(direcao);

                    // Aplica a rotação
                    transform.rotation = rotacaoDesejada;

                    // Depois gira -90 graus no eixo X
                    transform.Rotate(-90f, 0f, 0f, Space.Self);
                }
            }
        }
    }
}
