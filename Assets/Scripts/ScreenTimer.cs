using System;
using TMPro;
using UnityEngine;

public class ScreenTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField, Min(1)] private int startSeconds = 300;
    public event Action OnTimeUp;
    public float DurationSeconds { get; private set; }
    public float RemainingSeconds { get; private set; }
    public float ElapsedSeconds => DurationSeconds - RemainingSeconds;
    public bool IsPaused { get; private set; }
    public bool IsFinished { get; private set; }
    private CleanupManager cleanupManager;

    private void Awake()
    {
        DurationSeconds = Mathf.Max(1, startSeconds);
        RemainingSeconds = DurationSeconds;

        RefreshText();
    }

    private void Start()
    {
        cleanupManager = CleanupManager.Instance;

        if (cleanupManager != null)
            cleanupManager.OnRoomCleaned += Stop;
    }

    private void Update()
    {
        if (IsPaused || IsFinished)
            return;

        RemainingSeconds = Mathf.Max(0f, RemainingSeconds - Time.deltaTime);
        RefreshText();

        if (RemainingSeconds <= 0f)
        {
            IsFinished = true;

            if (timerText != null)
                timerText.text = "TIME UP";

            OnTimeUp?.Invoke();
        }
    }

    public void Pause()
    {
        IsPaused = true;
    }

    public void Resume()
    {
        IsPaused = false;
    }

    public void Stop()
    {
        if (IsPaused)
            return;
        
        IsFinished = true;
    }

    private void RefreshText()
    {
        if (timerText != null)
            timerText.text = FormatTime(RemainingSeconds);
    }

    public static string FormatTime(float seconds)
    {
        int totalSeconds = Mathf.CeilToInt(Mathf.Max(0f, seconds));
        return $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
    }

    private void OnDestroy()
    {
        if (cleanupManager != null)
            cleanupManager.OnRoomCleaned -= Stop;
    }
}