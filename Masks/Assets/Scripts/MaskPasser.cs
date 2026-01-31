using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaskPasser : MonoBehaviour
{
    public Transform player1Hand;
    public Transform player2Hand;

    [Header("Flight")]
    public float flightTime = 0.35f;   // kiek laiko skrenda
    public float arcHeight = 1.2f;     // arkos aukštis
    public float maxScaleMultiplier = 2f;

    [Header("Rotation in flight")]
    public float spinDegrees = 720f;            // kiek laipsniu apsisuka per visa skrydi (360 = 1x)
    public Vector3 spinAxis = Vector3.up;       // sukimosi asis
    public bool useWorldAxis = false;           // true = world, false = local

    private enum Holder { Player1, Player2 }
    [SerializeField] private Holder holder = Holder.Player1;

    private bool isFlying;

    void Start()
    {
        AttachToHolder();
    }

    void Update()
    {
        if (isFlying) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (holder == Holder.Player1 && kb.leftShiftKey.wasPressedThisFrame)
        {
            StartCoroutine(FlyTo(player2Hand, Holder.Player2));
            return;
        }

        if (holder == Holder.Player2 && kb.rightShiftKey.wasPressedThisFrame)
        {
            StartCoroutine(FlyTo(player1Hand, Holder.Player1));
            return;
        }
    }

    private void AttachToHolder()
    {
        Transform targetHand = (holder == Holder.Player1) ? player1Hand : player2Hand;
        transform.SetParent(targetHand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    private IEnumerator FlyTo(Transform target, Holder newHolder)
    {
        isFlying = true;

        transform.SetParent(null);

        Vector3 startPos = transform.position;
        Vector3 endPos = target.position;

        Vector3 baseScale = transform.localScale;
        transform.localScale = baseScale;

        Quaternion startRot = transform.rotation; // <- pradine rotacija

        float t = 0f;

        while (t < 1f)
        {
            float p = Mathf.Clamp01(t);

            Vector3 pos = Vector3.Lerp(startPos, endPos, p);

            float arc = 4f * p * (1f - p);
            pos.y += arc * arcHeight;

            transform.position = pos;

            float scaleMul = Mathf.Lerp(1f, maxScaleMultiplier, arc);
            transform.localScale = baseScale * scaleMul;

            float angleX = spinDegrees * p;
            transform.rotation = startRot * Quaternion.Euler(0f, 0f, angleX);

            yield return null;
            t += Time.deltaTime / Mathf.Max(0.0001f, flightTime);
        }

        holder = newHolder;
        AttachToHolder();

        isFlying = false;
    }

}
