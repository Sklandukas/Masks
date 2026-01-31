using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    private LaneStateManager laneStateManager;

    [Header("Game State")]
    public bool isGameOver;
    public bool hasGameStarted;

    [Header("UI")]
    [SerializeField] private GameObject startPanel;     
    [SerializeField] private GameObject gameOverPanel;  

    void Start()
    {
        laneStateManager = FindFirstObjectByType<LaneStateManager>();

        hasGameStarted = false;
        isGameOver = false;

        if (startPanel != null) startPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    void Update()
    {
        if (!hasGameStarted || isGameOver) return;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && laneStateManager != null)
        {
            laneStateManager.SetMaskState(!laneStateManager.isMaskOn);
        }
    }

    public void StartGame()
    {
        Debug.Log("StartGame() called");

        hasGameStarted = true;
        isGameOver = false;

        if (startPanel != null)
        {
            startPanel.SetActive(false);
            Debug.Log("startPanel active after SetActive(false)? " + startPanel.activeSelf);
        }
        else
        {
            Debug.LogError("startPanel is NULL in GameManagerScript Inspector!");
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void BeginGameOverSequence()
    {
        if (isGameOver) return; 
        isGameOver = true;
        Time.timeScale = 0f;
    }

    public void ShowGameOverUI()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("GAME OVER UI SHOWN");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
