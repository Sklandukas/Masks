using UnityEngine;

public class LevelMover : MonoBehaviour
{
    [Header("Settings")]
    public float movementSpeed = 5f;

    void Update()
    {
        transform.Translate(Vector3.up * movementSpeed * Time.deltaTime);
    }
}