using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject howToPlayPanel;

    private void Awake()
    {
        settingsPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
    }
    
    public void OnMenuButtonClicked(string buttonName)
    {
        Debug.Log($"Menu button clicked: {buttonName}");
        
        switch (buttonName)
        {
            case "Start":
                SceneManager.LoadScene("Room 1");
                break;

            case "Settings":
                settingsPanel.SetActive(true);
                break;
            
            case "HowToPlay":
                howToPlayPanel.SetActive(true);
                break;
            
            case "Quit":
                QuitGame();
                break;
        }
    }
    
    private static void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
    
    public void CloseHowToPlay()
    {
        howToPlayPanel.SetActive(false);
    }
}