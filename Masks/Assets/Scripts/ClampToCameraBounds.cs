using UnityEngine;

public class ClampToCameraBounds : MonoBehaviour
{
    [SerializeField] private Camera cam; // jei paliksi tuščią - ims Camera.main

    float halfW; // objekto pusplotis
    float halfH; // objekto pusaukštis

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        CacheObjectExtents();
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Kameros pasaulio ribos (apatinis kairys / viršutinis dešinys)
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0f, 0f, cam.nearClipPlane));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1f, 1f, cam.nearClipPlane));

        Vector3 p = transform.position;

        p.x = Mathf.Clamp(p.x, min.x + halfW, max.x - halfW);
        p.y = Mathf.Clamp(p.y, min.y + halfH, max.y - halfH);

        transform.position = p;
    }

    void CacheObjectExtents()
    {
        // Pirmiausia bandom SpriteRenderer
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            halfW = sr.bounds.extents.x;
            halfH = sr.bounds.extents.y;
            return;
        }

        // Tada Collider2D
        var c2d = GetComponent<Collider2D>();
        if (c2d != null)
        {
            halfW = c2d.bounds.extents.x;
            halfH = c2d.bounds.extents.y;
            return;
        }

        // Jei nieko nėra — laikom tašką
        halfW = 0f;
        halfH = 0f;
    }
}
