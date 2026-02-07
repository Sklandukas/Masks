using System.Collections;
using UnityEngine;

public class FinisDetections : MonoBehaviour
{
    private GameManagerScript gameManager;
    private string playerTag = "Player";
    private bool triggered = false;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManagerScript>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (!other.transform.root.CompareTag(playerTag)) return;

        triggered = true;

        gameManager?.BeginFinishSequence();

        var rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        other.enabled = false;
    }
}
