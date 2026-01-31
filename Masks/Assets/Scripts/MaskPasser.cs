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

    private enum Holder { Player1, Player2 }
    [SerializeField] private Holder holder = Holder.Player1;

    private bool isFlying;

    void Start()
    {
        // Pradžioje "prikabinam" prie turėtojo rankos
        AttachToHolder();
    }

    void Update()
    {
        if (isFlying) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        // Player1 turi objektą -> kairysis SHIFT meta pas Player2
        if (holder == Holder.Player1 && kb.leftShiftKey.wasPressedThisFrame)
        {
            StartCoroutine(FlyTo(player2Hand, Holder.Player2));
            return;
        }

        // Player2 turi objektą -> dešinysis SHIFT meta atgal pas Player1
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

        // atkabinam nuo rankos, kad galėtų judėti laisvai
        transform.SetParent(null);

        Vector3 startPos = transform.position;
        Vector3 endPos = target.position;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, flightTime);

            // Lerp tarp start/end
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);

            // Arka (parabolė): 4t(1-t) duoda gražų "kupolą"
            float height = 4f * t * (1f - t) * arcHeight;
            pos.y += height;

            transform.position = pos;
            yield return null;
        }

        // pabaigoje "prikabinam" prie gavėjo rankos
        holder = newHolder;
        AttachToHolder();

        isFlying = false;
    }
}
