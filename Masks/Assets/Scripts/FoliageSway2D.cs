using UnityEngine;

public class FoliageSway2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visual; // CHILD kuris suksis (Sprite)

    [Header("Idle wind sway")]
    [SerializeField] private float windAngle = 6f;      // bazinis siūbavimas (laipsniais)
    [SerializeField] private float windSpeed = 1.4f;    // siūbavimo greitis
    [SerializeField] private float randomPhase = 10f;   // kad visi nesisiūbuotų vienodai

    [Header("Top bends more than base")]
    [Tooltip("0 = beveik nejuda apačia, 1 = juda visas vienodai. Rekomenduoju 0.2-0.4")]
    [Range(0f, 1f)]
    [SerializeField] private float baseFlex = 0.25f;

    [Tooltip("Kiek aukštas tavo vizualas (Unity vienetais). Naudojama, kad viršus lankstytųsi labiau.")]
    [SerializeField] private float visualHeight = 1.0f;

    [Header("Player interaction")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float hitAngle = 10f;      // kiek papildomai užlenks nuo praeinančio
    [SerializeField] private float hitDamping = 6f;     // kaip greitai nuslopsta stuktelėjimas

    private float _phase;
    private float _hitOffset;      // laikinas papildomas kampas
    private float _hitVelocity;    // slopinimui

    private void Reset()
    {
        // patogu: auto priskirt child jei yra
        if (transform.childCount > 0) visual = transform.GetChild(0);
    }

    private void Awake()
    {
        if (visual == null)
            visual = transform;

        _phase = Random.Range(0f, randomPhase);
    }

    private void Update()
    {
        // 1) Vėjelis (sin)
        float wind = Mathf.Sin((Time.time + _phase) * windSpeed) * windAngle;

        // 2) Slopinam "hit" offsetą (paprastas critically damped spring)
        // velocity -> offset, kad gražiai sugrįžtų
        _hitVelocity = Mathf.Lerp(_hitVelocity, 0f, Time.deltaTime * hitDamping);
        _hitOffset = Mathf.Lerp(_hitOffset, 0f, Time.deltaTime * hitDamping);

        float totalAngle = wind + _hitOffset;

        // 3) „Viršus daugiau“ efektas:
        // vietoj to, kad viską suktume vienodai, mes:
        // - paliekam bazę beveik stabilią (baseFlex)
        // - vizualą papildomai paslenkam x kryptimi (shear) proporcingai aukščiui
        //
        // Vizualas sukasi mažiau, o viršus "išsineša" labiau per poslinkį.
        float rotAngle = totalAngle * baseFlex;
        visual.localRotation = Quaternion.Euler(0f, 0f, rotAngle);

        // poslinkis imituoja lenkimą: viršus juda labiau nei apačia
        // kuo didesnis kampas, tuo didesnis poslinkis į šoną
        float bendStrength = Mathf.Lerp(0.15f, 0.45f, 1f - baseFlex); // automatiškai
        float xOffset = Mathf.Sin(totalAngle * Mathf.Deg2Rad) * visualHeight * bendStrength;

        // paliekam originalų y (t.y. kad "nepakiltų")
        Vector3 p = visual.localPosition;
        p.x = xOffset;
        visual.localPosition = p;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // kryptis: jei žaidėjas iš kairės, lenkiam į dešinę (ir atvirkščiai)
        float dir = Mathf.Sign(transform.position.x - other.transform.position.x);

        // momentinis stuktelėjimas + truputį "velocity"
        _hitOffset += hitAngle * dir;
        _hitVelocity += hitAngle * dir;
    }
}
