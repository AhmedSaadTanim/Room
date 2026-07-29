using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BookSocketInteractor : XRSocketInteractor
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (args.interactableObject.transform.TryGetComponent(out CleanableObject cleanable))
        {
            cleanable.MarkCleaned();
        }
    }
}