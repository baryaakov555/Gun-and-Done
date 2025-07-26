using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("player")]
    public Transform playerTransform; // Reference to the player transform, needed for saving current position on game exit
    public Vector3 playerSpawnPosition; // Store the fixed spawn position of the player, used for game start or respawn
    public Vector3 playerSavedPosition; // Store the player's saved position when quitting the game

    [Header("Pause Menu")]
    public GameObject pauseMenuUI; // Reference to the pause menu UI, used for showing/hiding the menu when the game is paused

    [Header("Timer")]
    public TextMeshProUGUI timerText;

    [Header("Fall Counter")]
    public TextMeshProUGUI fallCounterText; // Reference to the UI text for displaying the fall counter

    private float timer;

    private int fallCounter;

    bool isGamePaused = false;

    private bool loadSave = false; // Flag to determine if the game should load a saved position

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        loadSave = PlayerPrefs.GetInt("LoadSave", 0) == 1; // Check if the game should load a saved position

        if (loadSave)
        {
            playerSavedPosition = new Vector3 (
                PlayerPrefs.GetFloat("PlayerPosX", 0f),
                PlayerPrefs.GetFloat("PlayerPosY", 0f),
                PlayerPrefs.GetFloat("PlayerPosZ", 0f)
            );
        }
    }

    void Start()
    {
        playerSpawnPosition = playerTransform.position; // remember spawn position

        if (loadSave && playerSavedPosition != Vector3.zero)
            playerTransform.position = playerSavedPosition; // Set the player's position to the saved position if it exists
        else
            playerTransform.position = playerSpawnPosition; // Set the player's position to the spawn position if no saved position exists

        SetPaused(false); // Ensure the game starts unpaused
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetPaused(!isGamePaused); // Toggle pause state when Escape is pressed

        if (Input.GetKeyDown(KeyCode.R))
            RestartGameButton(); // Restart the game when R is pressed

        if (!isGamePaused)
        {
            timer += Time.deltaTime; // Increment timer only when the game is not paused
            UpdateTimerUI(); // Update the timer UI with the current time
        }
    }

    public void SetPaused(bool paused)
    {
        isGamePaused = paused; // Set the paused state
        Time.timeScale = paused ? 0f : 1f; // Set time scale based on paused state

        if (pauseMenuUI)
            pauseMenuUI.SetActive(paused); // Show or hide the pause menu UI
    }

    public void ResumeGameButton() => SetPaused(false); // Resume the game by setting paused state to false

    public bool IsGamePaused() => isGamePaused; // Return the current paused state of the game

    public void RestartGameButton()
    {
        SetPaused(false); // Resume the game after restarting
        PlayerPrefs.DeleteKey("LoadSave"); // Delete the LoadSave key to ensure the game starts fresh next time
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); // Reload the current scene to restart the game
    }

    public void GoToMainMenuButton()
    {
        savePlayerPosition(); // Save the player's position before going to the main menu
        SetPaused(false); // Ensure the game is unpaused before going to the main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    void UpdateTimerUI()
    {
        int hours = Mathf.FloorToInt(timer / 3600f); // Calculate hours from timer
        int minutes = Mathf.FloorToInt((timer % 3600) / 60f); // Calculate minutes from timer
        int seconds = Mathf.FloorToInt(timer % 60f); // Calculate seconds from timer
        int milliseconds = Mathf.FloorToInt((timer *1000f) % 1000f); // Calculate milliseconds from timer
        timerText.text = $"{hours:00}:{minutes:00}:{seconds:00}.{milliseconds:000}"; // Update the timer UI text
    }

    public void IncrementFallCounterUI()
    {
        fallCounter++; // Increment the fall counter when the player falls
        UpdateFallCounterUI();
    }

    void UpdateFallCounterUI()
    {
        fallCounterText.text = $"Falls: {fallCounter}"; // Update the fall counter UI text
    }

    void OnApplicationQuit()
    {
        savePlayerPosition(); // Save the player's position when the application quits
    }

    void savePlayerPosition()
    {
        // Save the player's position when the game is closed
        playerSavedPosition = playerTransform.position;
        PlayerPrefs.SetFloat("PlayerPosX", playerSavedPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerSavedPosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", playerSavedPosition.z);
        PlayerPrefs.SetInt("LoadSave", 1); // Set LoadSave to true to indicate a saved position exists
        PlayerPrefs.Save();
    }

    void OnDisable()
    {
        if (Time.timeScale == 0f) Time.timeScale = 1f;
    }

}
