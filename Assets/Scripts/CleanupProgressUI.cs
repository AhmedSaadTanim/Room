using TMPro;
using UnityEngine;

public class CleanupProgressUI : MonoBehaviour
{
    [SerializeField] private TMP_Text progressText;

    private void Start()
    {
        CleanupManager.Instance.OnProgressChanged += UpdateUI;
        CleanupManager.Instance.OnRoomCleaned += RoomCleaned;
        UpdateUI(CleanupManager.Instance.CleanedObjects, CleanupManager.Instance.TotalObjects);
    }

    private void OnDestroy()
    {
        if (CleanupManager.Instance == null) return;
        CleanupManager.Instance.OnProgressChanged -= UpdateUI;
        CleanupManager.Instance.OnRoomCleaned -= RoomCleaned;
    }

    private void UpdateUI(int cleaned, int total)
        => progressText.text = $"Objects out of place: {total - cleaned} / {total}";

    private void RoomCleaned()
        => progressText.text = "Room cleaned!";
}