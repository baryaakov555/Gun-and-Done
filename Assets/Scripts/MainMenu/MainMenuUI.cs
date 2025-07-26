using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject continueButton; // Reference to the continue button in the main menu

    void Start()
    {
       if (PlayerPrefs.GetInt("LoadSave", 0) == 1)
            continueButton.SetActive(true); // Show continue button if a saved game exists
        else
            continueButton.SetActive(false); // Hide continue button if no saved game exists
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteKey("LoadSave"); // Delete LoadSave key to start a new game
        SceneManager.LoadScene("Gun and Done");
    }
    public void ContinueGame()
    {
        PlayerPrefs.SetInt("LoadSave", 1); // Set LoadSave to true
        SceneManager.LoadScene("Gun and Done");
    }
    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("Game closed!");
    }
}