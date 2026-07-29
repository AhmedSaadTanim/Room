using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CleanableObject : MonoBehaviour
{
    public bool IsCleaned { get; private set; }

    private XRGrabInteractable grab;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        if (grab != null)
            grab.selectEntered.AddListener(OnSelected);
    }

    private void OnDestroy()
    {
        if (grab != null)
            grab.selectEntered.RemoveListener(OnSelected);
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (!IsCleaned) return;

        // Ignore socket selecting it.
        if (args.interactorObject is XRSocketInteractor)
            return;

        MarkUncleaned();
    }

    public void MarkCleaned()
    {
        if (IsCleaned) return;

        IsCleaned = true;
        CleanupManager.Instance.IncrementProgress();
    }

    public void MarkUncleaned()
    {
        if (!IsCleaned) return;

        IsCleaned = false;
        CleanupManager.Instance.DecrementProgress();
    }
}