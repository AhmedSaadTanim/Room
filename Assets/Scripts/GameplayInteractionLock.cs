using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GameplayInteractionLock : MonoBehaviour, IXRSelectFilter
{
    [SerializeField] private XRInteractionManager interactionManager;

    [Tooltip("Movement, turning, teleportation and gravity providers only.")]
    [SerializeField] private Behaviour[] locomotionProviders;

    private bool locked;
    private bool registered;

    private readonly Dictionary<Behaviour, bool> providerStates = new();
    private readonly Dictionary<XRGrabInteractable, bool> throwStates = new();

    public bool canProcess => isActiveAndEnabled;

    private void Start()
    {
        interactionManager.selectFilters.Add(this);
        registered = true;
    }

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        if (!locked)
            return true;

        // Preserve objects already seated in sockets.
        if (interactor is XRSocketInteractor)
        {
            foreach (var selected in interactor.interactablesSelected)
            {
                if (ReferenceEquals(selected, interactable))
                    return true;
            }
        }

        return false;
    }

    public void SetLocked(bool value)
    {
        if (locked == value)
            return;

        locked = value;

        if (locked)
        {
            DisableLocomotion();
            ReleaseHeldObjects();
        }
        else
        {
            RestoreComponents();
        }
    }

    private void DisableLocomotion()
    {
        providerStates.Clear();

        foreach (var provider in locomotionProviders)
        {
            if (provider == null || providerStates.ContainsKey(provider))
                continue;

            providerStates.Add(provider, provider.enabled);
            provider.enabled = false;
        }
    }

    private void ReleaseHeldObjects()
    {
        var grabs = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);

        foreach (var grab in grabs)
        {
            var selectors =
                new List<IXRSelectInteractor>(grab.interactorsSelecting);

            foreach (var selector in selectors)
            {
                if (selector is XRSocketInteractor)
                    continue;

                if (!throwStates.ContainsKey(grab))
                    throwStates.Add(grab, grab.throwOnDetach);

                // Keep throwing disabled until the menu closes.
                grab.throwOnDetach = false;

                if (grab.interactionManager != null)
                    grab.interactionManager.SelectExit(selector, grab);
            }
        }
    }

    private void RestoreComponents()
    {
        foreach (var entry in providerStates)
        {
            if (entry.Key != null)
                entry.Key.enabled = entry.Value;
        }

        foreach (var entry in throwStates)
        {
            if (entry.Key != null)
                entry.Key.throwOnDetach = entry.Value;
        }

        providerStates.Clear();
        throwStates.Clear();
    }

    private void OnDestroy()
    {
        if (registered && interactionManager != null)
            interactionManager.selectFilters.Remove(this);

        RestoreComponents();
    }
}