using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaskPasser : MonoBehaviour
{
    public Transform player1Hand;
    public Transform player2Hand;

    [Header("Lane state (one manager per player level)")]
    [Tooltip("LaneStateManager on Player1's level. Mask on = true when mask is with Player1.")]
    public LaneStateManager laneManagerForPlayer1;
    [Tooltip("LaneStateManager on Player2's level. Mask on = true when mask is with Player2.")]
    public LaneStateManager laneManagerForPlayer2;

    [Header("Flight")]
    public float flightTime = 0.35f;   
    public float arcHeight = 1.2f;     
    public float maxScaleMultiplier = 2f;

    [Header("Rotation in flight")]
    public float spinDegrees = 360f;            
    public Vector3 spinAxis = Vector3.up;       
    public bool useWorldAxis = false;           

    private enum Holder { Player1, Player2 }
    [SerializeField] private Holder holder = Holder.Player1;

    private bool isFlying;

    void Start()
    {
        AttachToHolder(holder);
        UpdateLaneManagersForHolder(holder);
    }

    void Update()
    {
        if (isFlying) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (holder == Holder.Player1 && kb.leftShiftKey.wasPressedThisFrame)
        {
            RemoveMaskFromHolder(holder);
            StartCoroutine(FlyTo(player2Hand, Holder.Player2));
            return;
        }

        if (holder == Holder.Player2 && kb.rightShiftKey.wasPressedThisFrame)
        {
            RemoveMaskFromHolder(holder);
            StartCoroutine(FlyTo(player1Hand, Holder.Player1));
            return;
        }
    }

    private void AttachToHolder(Holder holder)
    {
        if (holder == Holder.Player1)
            AddMaskToHolder(holder);
        else if (holder == Holder.Player2)
            AddMaskToHolder(holder);
        else
            Debug.LogError($"[MaskPasser] Invalid holder: {holder}");
    }

    private IEnumerator FlyTo(Transform target, Holder newHolder)
    {
        isFlying = true;

        transform.SetParent(null);

        Vector3 startPos = transform.position;
        Vector3 endPos = target.position;

        Vector3 baseScale = transform.localScale;
        Quaternion startRot = transform.rotation;

        float elapsed = 0f;

        while (elapsed < flightTime)
        {
            float p = Mathf.Clamp01(elapsed / Mathf.Max(0.0001f, flightTime));

            Vector3 pos = Vector3.Lerp(startPos, endPos, p);

            float arc = 4f * p * (1f - p);
            pos.y += arc * arcHeight;

            transform.position = pos;

            float scaleMul = Mathf.Lerp(1f, maxScaleMultiplier, arc);
            transform.localScale = baseScale * scaleMul;

            float angleZ = spinDegrees * p;
            transform.rotation = startRot * Quaternion.Euler(0f, 0f, angleZ);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // final snap (kad 100% būtų tvarkingai)
        transform.position = endPos;
        transform.localScale = baseScale;
        transform.rotation = startRot * Quaternion.Euler(0f, 0f, spinDegrees);

        holder = newHolder;
        AttachToHolder(holder);
        UpdateLaneManagersForHolder(holder);

        isFlying = false;
    }

    void UpdateLaneManagersForHolder(Holder whoHasMask)
    {
        if (laneManagerForPlayer1 != null)
            laneManagerForPlayer1.SetMaskState(whoHasMask == Holder.Player1);
        if (laneManagerForPlayer2 != null)
            laneManagerForPlayer2.SetMaskState(whoHasMask == Holder.Player2);
    }


    void RemoveMaskFromHolder(Holder holder)
    {
        if (holder == Holder.Player1)
            laneManagerForPlayer1.SetMaskState(false);
        if (holder == Holder.Player2)
            laneManagerForPlayer2.SetMaskState(false);
    }

    void AddMaskToHolder(Holder holder)
    {
        Transform targetHand = (holder == Holder.Player1) ? player1Hand : player2Hand;
        transform.SetParent(targetHand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        if (holder == Holder.Player1)
            laneManagerForPlayer1.SetMaskState(true);
        if (holder == Holder.Player2)
            laneManagerForPlayer2.SetMaskState(true);
    }
}
