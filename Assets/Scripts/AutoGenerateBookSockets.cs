using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AutoGenerateBookSockets : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform booksParent;
    [SerializeField] private Transform socketStartPoint;

    [Header("Layout")]
    [SerializeField] private float extraGap = 0.01f;

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

            if (col != null)
            {
                socketObj.transform.rotation = col.transform.rotation;
                socketObj.transform.position = col.transform.TransformPoint(col.center);
            }
            else
            {
                socketObj.transform.rotation = book.rotation;
                socketObj.transform.position = book.position;
            }

            BoxCollider trigger = socketObj.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = col != null
                ? Vector3.Scale(col.size, col.transform.lossyScale)
                : book.lossyScale;

            BookSocketInteractor socket = socketObj.AddComponent<BookSocketInteractor>();
            socket.interactionLayers = bookInteractionLayer;
        }
    }
}