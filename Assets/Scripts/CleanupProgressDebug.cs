using UnityEngine;

public class CleanupProgressDebug : MonoBehaviour
{
    private void Start()
    {
        CleanupManager.Instance.OnProgressChanged += UpdateProgress;
        CleanupManager.Instance.OnRoomCleaned += RoomCleaned;
    }

    private void OnDestroy()
    {
        if (CleanupManager.Instance == null)
            return;

        CleanupManager.Instance.OnProgressChanged -= UpdateProgress;
        CleanupManager.Instance.OnRoomCleaned -= RoomCleaned;
    }

    private void UpdateProgress(int cleaned, int total)
    {
        Debug.Log($"Cleaned: {cleaned}/{total}");
    }

    private void RoomCleaned()
    {
        Debug.Log("ROOM CLEANED!");
    }
}