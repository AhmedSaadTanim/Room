using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AutoGenerateBookSockets : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform booksParent;
    [SerializeField] private Transform socketStartPoint;

    [Header("Socket Settings")]
    [SerializeField] private InteractionLayerMask bookInteractionLayer;

    private void Awake()
    {
        GenerateSockets();
    }

    private void Start()
    {
        CleanupManager.Instance.Initialize(booksParent.childCount);
    }

    private void GenerateSockets()
    {
        for (int i = 0; i < booksParent.childCount; i++)
        {
            Transform book = booksParent.GetChild(i);
            BoxCollider col = book.GetComponentInChildren<BoxCollider>();

            GameObject socketObj = new GameObject($"BookSocket_{i + 1}");
            socketObj.transform.SetParent(socketStartPoint, false);
            socketObj.transform.localScale = Vector3.one;

            // socket sits exactly where the book's pivot sits
            socketObj.transform.SetPositionAndRotation(book.position, book.rotation);

            BoxCollider trigger = socketObj.AddComponent<BoxCollider>();
            trigger.isTrigger = true;

            Vector3 parentScale = socketObj.transform.lossyScale;

            if (col != null)
            {
                // trigger box offset to the book's collider, relative to the pivot
                trigger.center = socketObj.transform.InverseTransformPoint(
                    col.transform.TransformPoint(col.center));

                Vector3 worldSize = Vector3.Scale(col.size, col.transform.lossyScale);

                trigger.size = new Vector3(
                    worldSize.x / Mathf.Max(0.0001f, parentScale.x),
                    worldSize.y / Mathf.Max(0.0001f, parentScale.y),
                    worldSize.z / Mathf.Max(0.0001f, parentScale.z)
                );
            }
            else
            {
                trigger.center = Vector3.zero;
                trigger.size = new Vector3(
                    book.lossyScale.x / Mathf.Max(0.0001f, parentScale.x),
                    book.lossyScale.y / Mathf.Max(0.0001f, parentScale.y),
                    book.lossyScale.z / Mathf.Max(0.0001f, parentScale.z)
                );
            }

            BookSocketInteractor socket = socketObj.AddComponent<BookSocketInteractor>();
            socket.interactionLayers = bookInteractionLayer;
            socket.AssignBook(book);
        }
    }
}