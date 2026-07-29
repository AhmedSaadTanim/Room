using TMPro;
using UnityEngine;

public class CleanupProgressUI : MonoBehaviour
{
    [SerializeField] private TMP_Text progressText;

    private void Start()
    {
        CleanupManager.Instance.OnProgressChanged += UpdateUI;
        CleanupManager.Instance.OnRoomCleaned += RoomCleaned;
    }

    private void RoomCleaned()
    {
        progressText.text =
            "Books Organized\n\n✓ Room Cleaned!";
    }

    private void OnDestroy()
    {
        if (CleanupManager.Instance != null)
            CleanupManager.Instance.OnProgressChanged -= UpdateUI;
    }

    private void UpdateUI(int cleaned, int total)
    {
        progressText.text = $"Books Organized\n{cleaned} / {total}";
    }
}