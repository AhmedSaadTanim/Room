using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameplayMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScreenTimer timer;
    [SerializeField] private GameObject menuWindow;
    [SerializeField] private FloatingMenuFollow menuFollow;
    
    [Header("Menu Content")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text timeLabelText;
    [SerializeField] private TMP_Text timeValueText;
    [SerializeField] private GameObject resumeButton;
    
    [Header("Input")]
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private GameObject interactionSimulator;
    [SerializeField] private GameplayInteractionLock interactionLock;

    [Header("Scenes")]
    [SerializeField] private string mainMenuScene = "Menu";

    private CleanupManager cleanupManager;
    private bool paused;
    private bool gameEnded;
    private bool enabledPauseAction;
    private float previousTimeScale = 1f;

    private void Awake()
    {
        menuWindow.SetActive(false);
    }

    private void Start()
    {
        cleanupManager = CleanupManager.Instance;

        if (cleanupManager != null)
            cleanupManager.OnRoomCleaned += HandleVictory;

        timer.OnTimeUp += HandleFailure;

        if (pauseAction != null && !pauseAction.action.enabled)
        {
            pauseAction.action.Enable();
            enabledPauseAction = true;
        }
    }

    private void Update()
    {
        if (gameEnded)
            return;
        
        bool usingSimulator = interactionSimulator != null &&
                              interactionSimulator.activeInHierarchy;

        bool pausePressed = usingSimulator ? Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame 
                                            : pauseAction != null && pauseAction.action.WasPressedThisFrame();
        
        if (pausePressed)
            TogglePause();
    }

    public void TogglePause()
    {
        if (gameEnded)
            return;
        
        if (paused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        if (paused || gameEnded || timer.IsFinished)
            return;

        paused = true;
        previousTimeScale = Time.timeScale;

        timer.Pause();
        Time.timeScale = 0f;
        interactionLock.SetLocked(true);

        titleText.text = "PAUSED";
        timeLabelText.text = "TIME REMAINING";
        timeValueText.text = ScreenTimer.FormatTime(timer.RemainingSeconds);
        
        menuWindow.SetActive(true);
        menuFollow.SnapToHead();
    }

    public void Resume()
    {
        if (!paused || gameEnded)
            return;

        paused = false;
        interactionLock.SetLocked(false);
        Time.timeScale = previousTimeScale;
        timer.Resume();

        menuWindow.SetActive(false);
        
        // Recheck progress in case a socket changed while paused.
        if (cleanupManager != null &&
            cleanupManager.TotalObjects > 0 &&
            cleanupManager.CleanedObjects >=
            cleanupManager.TotalObjects)
        {
            HandleVictory();
        }
    }

    private void HandleVictory()
    {
        if (gameEnded || paused)
            return;

        // If the deadline has already passed, timeout wins.
        if (timer.RemainingSeconds <= 0f)
        {
            HandleFailure();
            return;
        }

        EndChallenge(true);
    }
    
    private void HandleFailure()
    {
        if (gameEnded || paused)
            return;

        EndChallenge(false);
    }
    
    private void EndChallenge(bool won)
    {
        gameEnded = true;
        previousTimeScale = Time.timeScale;

        timer.Stop();
        Time.timeScale = 0f;
        interactionLock.SetLocked(true);

        titleText.text = won ? "COMPLETE!" : "TIME'S UP!";
        timeLabelText.text = won ? "COMPLETION TIME" : "TIME PLAYED";
        timeValueText.text = ScreenTimer.FormatTime(timer.ElapsedSeconds);

        resumeButton.SetActive(false);
        ShowMenu();
    }
    
    private void ShowMenu()
    {
        menuWindow.SetActive(true);
        menuFollow.SnapToHead();
    }
    
    public void Restart()
    {
        RestoreBeforeSceneChange();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        RestoreBeforeSceneChange();
        SceneManager.LoadScene(mainMenuScene);
    }

    private void RestoreBeforeSceneChange()
    {
        if (paused || gameEnded)
            Time.timeScale = previousTimeScale;

        paused = false;
        gameEnded = false;
    }

    private void OnDestroy()
    {
        if (cleanupManager != null)
            cleanupManager.OnRoomCleaned -= HandleVictory;

        if (timer != null)
            timer.OnTimeUp -= HandleFailure;

        if (paused || gameEnded)
            Time.timeScale = previousTimeScale;

        if (enabledPauseAction && pauseAction != null)
            pauseAction.action.Disable();
    }
}