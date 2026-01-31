using UnityEngine;
using UnityEngine.InputSystem;

public class GameManagerScript : MonoBehaviour
{
    private LaneStateManager laneStateManager;

    [Header("Game State")]
    public bool isGameOver;

    void Start()
    {
        laneStateManager = FindFirstObjectByType<LaneStateManager>();
        Time.timeScale = 1f;
        isGameOver = false;
    }

    void Update()
    {
        if (isGameOver) return;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && laneStateManager != null)
        {
            laneStateManager.SetMaskState(!laneStateManager.isMaskOn);
        }
    }

    public void BeginGameOverSequence()
    {
        isGameOver = true;
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        Debug.Log("GAME OVER");
    }

    public void ResumeGame()
    {
        isGameOver = false;
        Time.timeScale = 1f;
    }
}
