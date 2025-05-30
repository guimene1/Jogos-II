using UnityEngine;
using System.Collections;

public class SafeZoneTrigger : MonoBehaviour
{
    [Tooltip("Referência para o script KillerAI")]
    public KillerAI killerAI;

    [Tooltip("O assassino deve evitar permanentemente essa zona?")]
    public bool permanentSafeZone = true;

    [Tooltip("Tempo (em segundos) que o assassino evitará o jogador ao entrar na zona (se não for permanente)")]
    public float safeTimeDuration = 10f;

    private Coroutine safeTimeCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && killerAI != null)
        {
            killerAI.StopChase();
            killerAI.isInSafeZone = true;

            if (!permanentSafeZone)
            {
                if (safeTimeCoroutine != null)
                    StopCoroutine(safeTimeCoroutine);

                safeTimeCoroutine = StartCoroutine(TemporarySafeTime());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && killerAI != null)
        {
            if (!permanentSafeZone)
            {
                if (safeTimeCoroutine != null)
                {
                    StopCoroutine(safeTimeCoroutine);
                    safeTimeCoroutine = null;
                }

                killerAI.isInSafeZone = false;
            }
        }
    }

    private IEnumerator TemporarySafeTime()
    {
        yield return new WaitForSeconds(safeTimeDuration);

        if (killerAI != null)
        {
            killerAI.isInSafeZone = false;
        }

        safeTimeCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            if (collider is BoxCollider boxCollider)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawCube(boxCollider.center, boxCollider.size);
            }
            else if (collider is SphereCollider sphereCollider)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
            }
        }
    }
}
