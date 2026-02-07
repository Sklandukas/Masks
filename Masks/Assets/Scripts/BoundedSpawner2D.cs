using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BoundedSpawner2D : MonoBehaviour
{
    [Header("Optional: jei nori dėti tik ant tile centrų")]
    public Tilemap tilemap;
    public bool snapToTileCenters = true;

    [Header("Privaloma: ribų objektas (Collider2D)")]
    public Collider2D boundsCollider;

    [Header("Kur dėti sugeneruotus objektus")]
    public Transform parent;
    public bool clearPrevious = true;

    [Header("Prefab'ai")]
    public GameObject[] trees;
    public GameObject[] flowers;
    public GameObject[] holes;

    [Header("Seed")]
    public int seed = 12345;
    public bool useRandomSeedEachGenerate = true;

    [Header("Stilius")]
    public Vector2 fixedScaleXY = new Vector2(0.3f, 0.3f);

    [Header("PATCH (juostos/salos)")]
    [Tooltip("Kiek patchų (salų). Mažiau = daugiau tuštumos.")]
    public int patchCount = 8;

    [Tooltip("Minimalus atstumas tarp patch centrų.")]
    public float minPatchDistance = 10f;

    [Tooltip("Juostos ilgis (world units).")]
    public Vector2 patchLengthRange = new Vector2(10f, 16f);

    [Tooltip("Juostos plotis (world units).")]
    public Vector2 patchWidthRange = new Vector2(1.2f, 2.0f);

    [Header("Kiekiai vienam patch'ui")]
    public Vector2Int treesPerPatch = new Vector2Int(1, 2);
    public Vector2Int flowersPerPatch = new Vector2Int(2, 6);
    public Vector2Int holesPerPatch = new Vector2Int(0, 1);

    [Header("Atstumai")]
    public float minTreeDistance = 2.2f;

    [Header("Free placement (jei tilemap = null)")]
    public float freeGridStep = 0.8f;

    // vidiniai
    private Bounds spawnBounds;
    private readonly List<Vector2> patchCenters = new();
    private readonly List<Vector2> placedTreePositions = new();
    private readonly HashSet<Vector3Int> usedCells = new();

    void Start()
    {
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (boundsCollider == null)
        {
            Debug.LogError("Nepaskirtas boundsCollider!");
            return;
        }

        if (parent == null) parent = transform;

        if (clearPrevious) ClearPreviouslyGenerated();

        // ✅ kad kaskart būtų skirtingai
        if (useRandomSeedEachGenerate)
            seed = System.Environment.TickCount;

        Random.InitState(seed);

        spawnBounds = boundsCollider.bounds;
        patchCenters.Clear();
        placedTreePositions.Clear();
        usedCells.Clear();

        List<Vector3> candidates = CollectCandidates();
        if (candidates.Count == 0)
        {
            Debug.LogWarning("Nėra kandidatų ribose. Patikrink tilemap/boundsCollider.");
            return;
        }

        Shuffle(candidates);

        foreach (var c in candidates)
        {
            if (patchCenters.Count >= patchCount) break;
            if (!FarFromPatches(c)) continue;
            patchCenters.Add(c);
        }

        foreach (var center in patchCenters)
            SpawnPatch(center);

        Debug.Log($"Generated with seed: {seed}");
    }

    [ContextMenu("Randomize Seed + Generate")]
    public void RandomizeSeedAndGenerate()
    {
        seed = System.Environment.TickCount;
        useRandomSeedEachGenerate = false; // kad seed liktų toks, koks sugeneruotas
        Generate();
    }

    List<Vector3> CollectCandidates()
    {
        var list = new List<Vector3>(4096);

        if (tilemap != null)
        {
            BoundsInt b = tilemap.cellBounds;
            foreach (var cell in b.allPositionsWithin)
            {
                if (!tilemap.HasTile(cell)) continue;

                Vector3 p = tilemap.GetCellCenterWorld(cell);
                if (!boundsCollider.OverlapPoint(p)) continue;

                list.Add(p);
            }
        }
        else
        {
            for (float y = spawnBounds.min.y; y <= spawnBounds.max.y; y += freeGridStep)
            {
                for (float x = spawnBounds.min.x; x <= spawnBounds.max.x; x += freeGridStep)
                {
                    Vector3 p = new Vector3(x, y, 0f);
                    if (!boundsCollider.OverlapPoint(p)) continue;
                    list.Add(p);
                }
            }
        }

        return list;
    }

    void SpawnPatch(Vector3 center)
    {
        float length = Random.Range(patchLengthRange.x, patchLengthRange.y);
        float width = Random.Range(patchWidthRange.x, patchWidthRange.y);

        float angle = Random.Range(0f, 360f);
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x);

        int tCount = Random.Range(treesPerPatch.x, treesPerPatch.y + 1);
        for (int i = 0; i < tCount; i++)
        {
            Vector3 pos = PointOnPatch(center, dir, perp, length, width);
            if (!ValidatePos(ref pos)) continue;
            if (!FarFromTrees(pos)) continue;

            PlaceRandom(trees, pos);
            placedTreePositions.Add(pos);
        }

        int fCount = Random.Range(flowersPerPatch.x, flowersPerPatch.y + 1);
        for (int i = 0; i < fCount; i++)
        {
            Vector3 pos = PointOnPatch(center, dir, perp, length, width);
            if (!ValidatePos(ref pos)) continue;

            PlaceRandom(flowers, pos);
        }

        int hCount = Random.Range(holesPerPatch.x, holesPerPatch.y + 1);
        for (int i = 0; i < hCount; i++)
        {
            Vector3 pos = PointOnPatch(center, dir, perp, length * 0.8f, width * 0.8f);
            if (!ValidatePos(ref pos)) continue;

            PlaceRandom(holes, pos);
        }
    }

    Vector3 PointOnPatch(Vector3 center, Vector2 dir, Vector2 perp, float length, float width)
    {
        float t = Random.Range(-length * 0.5f, length * 0.5f);
        float u = Random.Range(-width * 0.5f, width * 0.5f);
        Vector2 p = (Vector2)center + dir * t + perp * u;
        return new Vector3(p.x, p.y, 0f);
    }

    bool ValidatePos(ref Vector3 pos)
    {
        if (!boundsCollider.OverlapPoint(pos)) return false;

        if (tilemap != null && snapToTileCenters)
        {
            pos = SnapToTileCenter(pos);
            if (!boundsCollider.OverlapPoint(pos)) return false;

            Vector3Int cell = tilemap.WorldToCell(pos);
            if (usedCells.Contains(cell)) return false;
            usedCells.Add(cell);
        }

        return true;
    }

    bool FarFromPatches(Vector2 pos)
    {
        for (int i = 0; i < patchCenters.Count; i++)
            if (Vector2.Distance(pos, patchCenters[i]) < minPatchDistance)
                return false;
        return true;
    }

    bool FarFromTrees(Vector2 pos)
    {
        for (int i = 0; i < placedTreePositions.Count; i++)
            if (Vector2.Distance(pos, placedTreePositions[i]) < minTreeDistance)
                return false;
        return true;
    }

    void PlaceRandom(GameObject[] prefabs, Vector3 worldPos)
    {
        if (prefabs == null || prefabs.Length == 0) return;

        var prefab = prefabs[Random.Range(0, prefabs.Length)];
        var go = Instantiate(prefab);

        go.transform.position = worldPos;
        go.transform.rotation = Quaternion.identity; // 0,0,0
        go.transform.localScale = new Vector3(fixedScaleXY.x, fixedScaleXY.y, 1f); // x,y=0.3

        if (parent != null)
            go.transform.SetParent(parent, true);
    }

    Vector3 SnapToTileCenter(Vector3 worldPos)
    {
        Vector3Int cell = tilemap.WorldToCell(worldPos);
        return tilemap.GetCellCenterWorld(cell);
    }

    void ClearPreviouslyGenerated()
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
