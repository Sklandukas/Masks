using UnityEngine;
using UnityEngine.InputSystem;

public class GameManagerScript : MonoBehaviour
{
    private LaneStateManager laneStateManager;

    void Start()
    {
        laneStateManager = FindFirstObjectByType<LaneStateManager>();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && laneStateManager != null)
        {
            laneStateManager.SetMaskState(!laneStateManager.isMaskOn);
        }
    }
}
