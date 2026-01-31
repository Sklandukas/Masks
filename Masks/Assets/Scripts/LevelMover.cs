using UnityEngine;

public class LevelMover : MonoBehaviour
{
    [Header("Settings")]
    public float scrollSpeed = 5f;

    void Update()
    {
        // Move the object down along the Y-axis
        // Use Vector3.up if you want the level to move UP (player falls)
        // Use Vector3.down if you want the level to move DOWN (player flies up)
        
        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
    }
}