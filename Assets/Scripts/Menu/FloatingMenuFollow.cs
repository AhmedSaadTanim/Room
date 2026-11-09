using UnityEngine;

public class FloatingMenuFollow : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private GameObject menuWindow;

    [Header("Placement")]
    [SerializeField] private float distance = 1.2f;
    [SerializeField] private float verticalOffset = -0.05f;

    [Header("Following")]
    [SerializeField] private float turnThreshold = 20f;
    [SerializeField] private float moveThreshold = 0.2f;
    [SerializeField] private float followSpeed = 8f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 lastHeadPosition;
    private Vector3 lastForward;
    private bool initialized;

    private void LateUpdate()
    {
        if (head == null || menuWindow == null)
            return;

        if (!menuWindow.activeInHierarchy)
        {
            initialized = false;
            return;
        }

        if (!initialized)
            SnapToHead();

        Vector3 forward = GetFlatForward();
        bool turned = Vector3.Angle(lastForward, forward) > turnThreshold;
        bool moved = Vector3.Distance(lastHeadPosition, head.position) > moveThreshold;

        if (turned || moved)
            UpdateTarget();

        float blend = 1f - Mathf.Exp(-followSpeed * Time.unscaledDeltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPosition, blend);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, blend);
    }

    public void SnapToHead()
    {
        if (head == null)
            return;

        UpdateTarget();
        transform.SetPositionAndRotation(targetPosition, targetRotation);
        initialized = true;
    }

    private void UpdateTarget()
    {
        lastForward = GetFlatForward();
        lastHeadPosition = head.position;
        targetPosition = head.position + lastForward * distance + Vector3.up * verticalOffset;
        targetRotation = Quaternion.LookRotation(lastForward, Vector3.up);
    }

    private Vector3 GetFlatForward()
    {
        Vector3 forward = Vector3.ProjectOnPlane(head.forward, Vector3.up);

        if (forward.sqrMagnitude < 0.001f)
        {
            return lastForward.sqrMagnitude > 0.001f ? lastForward : Vector3.forward;
        }

        return forward.normalized;
    }
}