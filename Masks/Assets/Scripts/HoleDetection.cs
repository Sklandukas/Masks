using System.Collections;
using UnityEngine;

public class HoleDetecion : MonoBehaviour
{
    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private string playerTag = "Player";

    [Header("Disappear Animation")]
    [SerializeField] private float popUpScalePercent = 0.10f;
    [SerializeField] private float popDuration = 0.12f;
    [SerializeField] private float shrinkDuration = 0.35f;
    [SerializeField] private float holdDuration = 0.10f;
    [SerializeField] private float spinDegrees = 720f;

    private bool triggered;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManagerScript>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (!other.transform.root.CompareTag(playerTag)) return;

        triggered = true;

        gameManager?.BeginGameOverSequence();
        gameManager?.GameOver(); 

        var rb = other.attachedRigidbody; 
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;   
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        other.enabled = false;

        StartCoroutine(AnimateThenDisappear(other.transform, other.gameObject));
    }

    private IEnumerator AnimateThenDisappear(Transform target, GameObject targetGO)
    {
        Renderer rend = target.GetComponentInChildren<Renderer>();
        Vector3 lockedVisualCenter = rend != null ? rend.bounds.center : target.position;

        Vector3 startScale = target.localScale;
        Vector3 popScale = startScale * (1f + popUpScalePercent);

        Quaternion startRot = target.rotation;

        float t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = popDuration <= 0f ? 1f : Mathf.Clamp01(t / popDuration);

            target.rotation = startRot;
            target.localScale = Vector3.Lerp(startScale, popScale, k);

            KeepVisualCenterFixed(target, rend, lockedVisualCenter);

            yield return null;
        }

        if (holdDuration > 0f)
        {
            target.rotation = startRot;
            target.localScale = popScale;
            KeepVisualCenterFixed(target, rend, lockedVisualCenter);

            yield return new WaitForSecondsRealtime(holdDuration);
        }

        t = 0f;
        while (t < shrinkDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = shrinkDuration <= 0f ? 1f : Mathf.Clamp01(t / shrinkDuration);

            float z = spinDegrees * k;
            target.rotation = startRot * Quaternion.Euler(0f, 0f, z);
            target.localScale = Vector3.Lerp(popScale, Vector3.zero, k);

            KeepVisualCenterFixed(target, rend, lockedVisualCenter);

            yield return null;
        }

        Destroy(targetGO);
    }

    private void KeepVisualCenterFixed(Transform target, Renderer rend, Vector3 lockedCenter)
    {
        if (rend == null)
        {
            return;
        }

        Vector3 currentCenter = rend.bounds.center;

        Vector3 delta = lockedCenter - currentCenter;

        target.position += delta;
    }
}
