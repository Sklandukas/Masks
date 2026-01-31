using System.Collections;
using UnityEngine;

public class FoliageWiggle : MonoBehaviour
{
    [Header("Wiggle")]
    [SerializeField] private float maxAngle = 10f;   // kiek pasisuks
    [SerializeField] private float duration = 0.15f; // kiek truks pasisukimas

    [Header("Who can trigger")]
    [SerializeField] private string playerTag = "Player";

    private Quaternion startRot;
    private Coroutine co;

    private void Awake()
    {
        startRot = transform.localRotation;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER2D: " + other.name);

        if (!other.CompareTag(playerTag)) return;

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Wiggle(other.transform));
    }

    private IEnumerator Wiggle(Transform passer)
    {
        // kryptis nuo žaidėjo iki augalo (2D: XY plokštuma)
        Vector2 dir = (Vector2)(transform.position - passer.position);
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;
        dir.Normalize();

        // 2D sukam aplink Z ašį: jei žaidėjas iš kairės – sukam viena kryptim, iš dešinės – kita
        float sign = Mathf.Sign(Vector2.Dot(dir, Vector2.right));
        Quaternion target = startRot * Quaternion.AngleAxis(maxAngle * sign, Vector3.forward);

        // į šoną
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float ease = Mathf.SmoothStep(0f, 1f, t);
            transform.localRotation = Quaternion.Slerp(startRot, target, ease);
            yield return null;
        }

        // atgal
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (duration * 1.2f);
            float ease = Mathf.SmoothStep(0f, 1f, t);
            transform.localRotation = Quaternion.Slerp(target, startRot, ease);
            yield return null;
        }

        transform.localRotation = startRot;
        co = null;
    }
}
