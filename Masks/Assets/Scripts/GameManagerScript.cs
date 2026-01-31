using UnityEngine;
using UnityEngine.InputSystem;

public class GameManagerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Find ALL obstacles and tell them to switch to Alternate
            // (Note: In a real game, you'd use a more efficient list, not FindObjects)
            var allObstacles = FindObjectsByType<ObstacleBehaviour>(FindObjectsSortMode.None);
            
            foreach(var obs in allObstacles)
            {
                obs.UpdateObstacleState(ObstacleBehaviour.GameState.SuKauke);
            }
        }
        
    }
}
