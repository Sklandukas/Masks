using UnityEngine;

public class GrassTouchReactor : MonoBehaviour
{
    [Header("Settings")]
    public float normalSway = 0.1f;
    public float disturbedSway = 0.3f;
    
    public float normalSpeed = 2f;
    public float disturbedSpeed = 3f;

    [Tooltip("How fast it returns to normal (Higher = Faster)")]
    public float recoverySpeed = 0.5f;

    // Internal state (0 = Calm, 1 = Fully Disturbed)
    private float currentDisturbance = 0f;
    private bool isDisturbed = false;
    
    private SpriteRenderer sr;
    private MaterialPropertyBlock propBlock;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // When Player hits the trigger, max out the disturbance
        if (other.CompareTag("Player"))
        {
            currentDisturbance = 4f;
            isDisturbed = true;
            Disturb();
        }
    }

    void Disturb()
    {
        // set material properties to disturbed values
        sr.GetPropertyBlock(propBlock); // Load current block
        propBlock.SetFloat("_Amount", disturbedSway);
        propBlock.SetFloat("_Speed", disturbedSpeed);
        sr.SetPropertyBlock(propBlock); // Send changes back
        Debug.Log("Disturbed");
    }

    void Calm()
    {
        // set material properties to normal values
        sr.GetPropertyBlock(propBlock); // Load current block
        propBlock.SetFloat("_Amount", normalSway);
        propBlock.SetFloat("_Speed", normalSpeed);
        sr.SetPropertyBlock(propBlock); // Send changes back
        Debug.Log("Calmed");
    }

    void Update()
    {
        if (isDisturbed)
        {
            // reduce disturbance over time
            currentDisturbance -= Time.deltaTime * recoverySpeed;
            if (currentDisturbance <= 0)
            {
                isDisturbed = false;
                currentDisturbance = 0f;
                Calm();
            }
        }
    }
}