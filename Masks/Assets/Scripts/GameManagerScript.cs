using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    private LaneStateManager laneStateManager;

    [Header("Game State")]
    public bool isGameOver;
    public bool isFinish;
    public bool hasGameStarted;

    [Header("UI")]
    [SerializeField] private GameObject startPanel;     
    [SerializeField] private GameObject gameOverPanel;  
    [SerializeField] private GameObject FinishPanel; 

    [SerializeField] private GameObject objectToHide; 


        
    void Start()
    {
        laneStateManager = FindFirstObjectByType<LaneStateManager>();

        hasGameStarted = false;
        isGameOver = false;
        isFinish = false;

        if (objectToHide != null) objectToHide.SetActive(false);
        if (startPanel != null) startPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    void Update()
    {
        // 1. Patikra pradžiai (jei žaidimas dar neprasidėjo)
        if (!hasGameStarted)
        {
            if (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
            {
                StartGame();
                return; // Sustabdome Update šiam kadrui, kad netyčia nesuveiktų kita logika
            }
        }

        // 2. Patikra restartui (jei žaidimas jau pasibaigęs)
        if (isGameOver || isFinish)
        {
            if (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
            {
                RestartGame();
                return;
            }
        }

        // 3. Jei žaidimas nevyksta (pauzė arba pabaiga), toliau nieko nedarom
        if (!hasGameStarted || isGameOver || isFinish) return;

        // 4. Žaidimo eigos valdymas (pvz., kaukė su Space)
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
        isFinish = false;

        if (objectToHide != null) objectToHide.SetActive(true);

        if (startPanel != null) startPanel.SetActive(false);

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

        if (FinishPanel != null) FinishPanel.SetActive(false); // Pridėk šitą
            Time.timeScale = 0f;

        Time.timeScale = 1f;
    }

    public void BeginGameOverSequence()
    {
        if (isGameOver) return; 
        isGameOver = true;
        Time.timeScale = 0f;
    }

    public void BeginFinishSequence()
    {
        if(isFinish) return;
        isFinish = true;
        Time.timeScale = 0f;
        ShowFinishUI(); // Iškviečiame UI automatiškai
    }

    public void ShowGameOverUI()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("GAME OVER UI SHOWN");
    }

    public void ShowFinishUI()
    {
        if(FinishPanel != null)
        {
            FinishPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting Game..."); // Patikrink Console langą žaisdamas
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}
