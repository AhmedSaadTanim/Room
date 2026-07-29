using System;
using UnityEngine;

public class CleanupManager : MonoBehaviour
{
    public static CleanupManager Instance { get; private set; }

    public event Action<int, int> OnProgressChanged;
    public event Action OnRoomCleaned;

    private int cleanedObjects;
    private int totalObjects;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Initialize(int total)
    {
        totalObjects = total;
        cleanedObjects = 0;

        OnProgressChanged?.Invoke(cleanedObjects, totalObjects);
    }

    public void IncrementProgress()
    {
        cleanedObjects++;

        OnProgressChanged?.Invoke(cleanedObjects, totalObjects);

        if (cleanedObjects >= totalObjects)
        {
            OnRoomCleaned?.Invoke();
        }
    }

    public void DecrementProgress()
    {
        cleanedObjects--;

        cleanedObjects = Mathf.Max(0, cleanedObjects);

        OnProgressChanged?.Invoke(cleanedObjects, totalObjects);
    }
}