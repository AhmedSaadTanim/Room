using UnityEngine;
using System.Collections;
using TMPro;

public class ScreenTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private int startSeconds = 300;

    private bool stopped;

    private void Start()
    {
        CleanupManager.Instance.OnRoomCleaned += Stop;
        StartCoroutine(Tick());
    }

    private void OnDestroy()
    {
        if (CleanupManager.Instance != null)
            CleanupManager.Instance.OnRoomCleaned -= Stop;
    }

    private void Stop() => stopped = true;

    private IEnumerator Tick()
    {
        var wait = new WaitForSeconds(1f);
        for (int t = startSeconds; t >= 0 && !stopped; t--)
        {
            timerText.text = $"{t / 60:00}:{t % 60:00}";
            yield return wait;
        }

        if (!stopped) timerText.text = "TIME UP";
    }
}