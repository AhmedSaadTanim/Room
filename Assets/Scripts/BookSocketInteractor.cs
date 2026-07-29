using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BookSocketInteractor : XRSocketInteractor
{
    private Transform assignedBook;

    public void AssignBook(Transform book)
    {
        assignedBook = book;
    }

    private bool IsAssigned(IXRInteractable interactable)
    {
        if (assignedBook == null) return true;

        Transform t = interactable.transform;
        return t == assignedBook || t.IsChildOf(assignedBook);
    }

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && IsAssigned(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && IsAssigned(interactable);
    }
}