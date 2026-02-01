using UnityEngine;

public class ObstacleBehaviour : MonoBehaviour
{
    [Header("Designer Settings")]
    [Tooltip("Sprite shown when mask is OFF (normal mode).")]
    public Sprite normalSprite;
    [Tooltip("Sprite shown when mask is ON (masked version).")]
    public Sprite alternateSprite;
    [Tooltip("If false, this obstacle is hidden completely when mask is ON (no sprite, no collision).")]
    public bool existsInAlternate = true;

    private SpriteRenderer myRenderer;
    private Collider2D myCollider;
    private LaneStateManager myLaneManager;
    private bool hit = false;

    void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        Debug.Log($"[ObstacleBehaviour] '{gameObject.name}' started");
        myLaneManager = GetComponentInParent<LaneStateManager>();
        if (myLaneManager != null)
            StartListeningToLaneManager();
        else
            Debug.LogError($"[ObstacleBehaviour] '{gameObject.name}' is missing LaneStateManager. Assign one in the Inspector.", this);
    }

    void StartListeningToLaneManager()
    {
        Debug.Log($"[ObstacleBehaviour] '{gameObject.name}' started listening to LaneStateManager");
        myLaneManager.OnMaskStateChanged += UpdateVisuals;
        UpdateVisuals(myLaneManager.isMaskOn);
    }

    void OnDestroy()
    {
        StopListeningToLaneManager();
    }

    void StopListeningToLaneManager()
    {
        if (myLaneManager != null)
        {
            myLaneManager.OnMaskStateChanged -= UpdateVisuals;
        }
    }

    void UpdateVisuals(bool maskActive)
    {
        Debug.Log($"[ObstacleBehaviour] '{gameObject.name}' updated visuals to {maskActive}");
        if (maskActive || hit)
        {
            if (existsInAlternate)
            {
                myRenderer.sprite = alternateSprite;
                myRenderer.enabled = true;
                myCollider.enabled = true;
            }
            else
            {
                myRenderer.enabled = false;
                myCollider.enabled = false;
            }
        }
        else
        {
            myRenderer.sprite = normalSprite;
            myRenderer.enabled = true;
            myCollider.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hit = true;
            UpdateVisuals(true);
            Debug.Log("Player hit " + gameObject.name);
        }
    }
}
