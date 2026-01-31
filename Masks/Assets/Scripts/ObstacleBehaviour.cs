using UnityEngine;

public class ObstacleBehaviour : MonoBehaviour
{
    // 1. Define the possible states (You can add more later)
    public enum GameState { BeKaukes, SuKauke }

    [Header("Designer Setup")]
    public Sprite beKaukes;      // Drag the "Standard" look here
    public Sprite suKauke;   // Drag the "Spooky/Hidden" look here

    [Header("Colour Fallback (used when sprite not set)")]
    [Tooltip("Colour when state is BeKaukes. Used only if beKaukes is not assigned.")]
    public Color spalveBeKaukes = Color.white;
    [Tooltip("Colour when state is SuKauke. Used only if suKauke is not assigned.")]
    public Color spalveSuKauke = Color.gray;

    [Tooltip("If false, the object vanishes completely in SuKauke state")]
    public bool existsInSuKauke = true;

    private SpriteRenderer myRenderer;
    private Collider2D myCollider;

    void Awake()
    {
        // Grab references to the components on this object
        myRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        // Set default look
        UpdateObstacleState(GameState.BeKaukes);
    }

    // Call this function from your GameManager whenever state changes!
    public void UpdateObstacleState(GameState newState)
    {
        if (newState == GameState.BeKaukes)
        {
            if (beKaukes != null)
            {
                myRenderer.sprite = beKaukes;
                myRenderer.color = Color.white;
            }
            else
            {
                myRenderer.color = spalveBeKaukes;
            }
            myRenderer.enabled = true;
            myCollider.enabled = true;
        }
        else if (newState == GameState.SuKauke)
        {
            if (existsInSuKauke)
            {
                if (suKauke != null)
                {
                    myRenderer.sprite = suKauke;
                    myRenderer.color = spalveSuKauke;
                }
                else
                {
                    myRenderer.color = spalveSuKauke;
                }
            }
            else
            {
                // Disable it completely (invisible and no collision)
                myRenderer.enabled = false;
                myCollider.enabled = false;
            }
        }
    }
    
    // Logic for hitting the player
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player hit " + gameObject.name);
            // Add your damage logic here later
        }
    }
}