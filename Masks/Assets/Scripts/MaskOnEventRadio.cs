using UnityEngine;
using System; // Required for 'Action' (Events)

public class LaneStateManager : MonoBehaviour
{
    // The current state
    public bool isMaskOn = false;

    // The "Broadcast" event. Obstacles will subscribe to this.
    // 'Action<bool>' means we send a true/false value with the message.
    public event Action<bool> OnMaskStateChanged;

    // Call this from your Player script when they press the button
    public void SetMaskState(bool active)
    {
        // Only broadcast if the value actually changes
        if (isMaskOn != active)
        {
            isMaskOn = active;
            
            // Send the signal to anyone listening!
            OnMaskStateChanged?.Invoke(isMaskOn);
        }
    }
}