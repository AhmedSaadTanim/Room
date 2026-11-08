using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    private void Awake()
    {
        settingsPanel.SetActive(false);
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
            
            case "Quit":
                Application.Quit();
                break;
        }
    }
    
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}