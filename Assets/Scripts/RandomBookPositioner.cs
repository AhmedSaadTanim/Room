using UnityEngine;

public class RandomBookPositioner : MonoBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private BoxCollider spawnArea;

    [Header("Books")]
    [SerializeField] private Transform booksParent;
    private Transform[] books;

    [Header("Surface Detection")]
    [SerializeField] private LayerMask surfaceMask;
    [SerializeField] private float spawnLift = 0.03f;
    [SerializeField] private float maxSurfaceAngle = 20f;
    [SerializeField] private float edgeMarginMultiplier = 1f;

    [Header("Natural Look")]
    [SerializeField, Range(0f, 1f)] private float uprightChance = 0.15f;
    [SerializeField] private float tiltRange = 5f;

    [Header("Collision Check")]
    [SerializeField] private Vector3 fallbackCheckSize = new Vector3(0.35f, 0.15f, 0.25f);
    [SerializeField] private float checkPadding = 0.02f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private int maxAttemptsPerBook = 50;

    private readonly Collider[] overlapResults = new Collider[16];

    private void Start()
    {
        FindBooks();
        RandomizeBooks();
    }

    private void FindBooks()
    {
        books = new Transform[booksParent.childCount];

        for (int i = 0; i < booksParent.childCount; i++)
            books[i] = booksParent.GetChild(i);
    }

    public void RandomizeBooks()
    {
        for (int i = 0; i < books.Length; i++)
        {
            Transform book = books[i];
            Vector3 startPos = book.position;
            Quaternion startRot = book.rotation;

            BoxCollider col = book.GetComponentInChildren<BoxCollider>();
            Vector3 bookSize = GetBookSize(col);
            float margin = Mathf.Max(bookSize.x, bookSize.y, bookSize.z) * edgeMarginMultiplier;

            Quaternion flatBase = GetFlatBaseRotation(book, col);

            bool placed = false;

            for (int attempt = 0; attempt < maxAttemptsPerBook; attempt++)
            {
                Vector3 rayStart = GetRandomPointAtAreaTop(margin);
                float rayLength = spawnArea.size.y * spawnArea.transform.lossyScale.y * 1.5f;

                if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, rayLength,
                                     surfaceMask, QueryTriggerInteraction.Ignore))
                    continue;

                if (Vector3.Angle(hit.normal, Vector3.up) > maxSurfaceAngle)
                    continue;

                Quaternion rot = GetNaturalRotation(flatBase);

                PlaceAtTarget(book, col, hit.point, rot);

                if (IsBlocked(book, col))
                    continue;

                FinalizeBook(book);
                placed = true;
                break;
            }

            if (!placed)
            {
                ApplyTransform(book, startPos, startRot);
                Debug.LogWarning($"Could not find valid surface for {book.name}");
            }
        }
    }

    // Rotation that lays the book flat: its thinnest axis points straight up.
    private Quaternion GetFlatBaseRotation(Transform book, BoxCollider col)
    {
        if (col == null)
            return book.rotation;

        Vector3 size = Vector3.Scale(col.size, col.transform.lossyScale);

        Vector3 thinAxisLocal;
        if (size.x <= size.y && size.x <= size.z) thinAxisLocal = Vector3.right;
        else if (size.y <= size.x && size.y <= size.z) thinAxisLocal = Vector3.up;
        else thinAxisLocal = Vector3.forward;

        Vector3 thinWorld = col.transform.rotation * thinAxisLocal;
        Vector3 thinInBookSpace = Quaternion.Inverse(book.rotation) * thinWorld;

        return Quaternion.FromToRotation(thinInBookSpace, Vector3.up);
    }

    private Quaternion GetNaturalRotation(Quaternion flatBase)
    {
        Quaternion pose = Random.value < uprightChance
            ? Quaternion.AngleAxis(90f, Vector3.right) * flatBase
            : flatBase;

        Quaternion spin = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        Quaternion tilt = Quaternion.Euler(
            Random.Range(-tiltRange, tiltRange),
            0f,
            Random.Range(-tiltRange, tiltRange)
        );

        return tilt * spin * pose;
    }

    private void PlaceAtTarget(Transform book, BoxCollider col, Vector3 target, Quaternion rot)
    {
        if (col == null)
        {
            ApplyTransform(book, target + Vector3.up * (fallbackCheckSize.y * 0.5f + spawnLift), rot);
            return;
        }

        ApplyTransform(book, book.position, rot);

        Vector3 pivotToColliderCentre = col.bounds.center - book.position;
        float halfHeight = col.bounds.extents.y;

        Vector3 desiredCentre = new Vector3(
            target.x,
            target.y + halfHeight + spawnLift,
            target.z
        );

        ApplyTransform(book, desiredCentre - pivotToColliderCentre, rot);
    }

    private bool IsBlocked(Transform book, BoxCollider col)
    {
        Vector3 center = col != null ? col.bounds.center : book.position;
        Vector3 halfExtents = (GetBookSize(col) + Vector3.one * checkPadding) * 0.5f;

        int count = Physics.OverlapBoxNonAlloc(center, halfExtents, overlapResults,
                                               book.rotation, obstacleMask,
                                               QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            if (overlapResults[i].transform.IsChildOf(book)) continue;
            return true;
        }

        return false;
    }

    private Vector3 GetBookSize(BoxCollider col)
    {
        return col != null
            ? Vector3.Scale(col.size, col.transform.lossyScale)
            : fallbackCheckSize;
    }

    private void ApplyTransform(Transform book, Vector3 position, Quaternion rotation)
    {
        book.SetPositionAndRotation(position, rotation);
        Physics.SyncTransforms();
    }

    private void FinalizeBook(Transform book)
    {
        Rigidbody rb = book.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.maxDepenetrationVelocity = 1f;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.WakeUp();
        }
    }

    private Vector3 GetRandomPointAtAreaTop(float margin)
    {
        float sizeX = Mathf.Max(0f, spawnArea.size.x - margin * 2f);
        float sizeZ = Mathf.Max(0f, spawnArea.size.z - margin * 2f);

        Vector3 local = spawnArea.center + new Vector3(
            Random.Range(-0.5f, 0.5f) * sizeX,
            spawnArea.size.y * 0.5f,
            Random.Range(-0.5f, 0.5f) * sizeZ
        );

        return spawnArea.transform.TransformPoint(local);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnArea == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.matrix = spawnArea.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(spawnArea.center, spawnArea.size);
    }
}