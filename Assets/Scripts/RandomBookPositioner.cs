using UnityEngine;

public class RandomBookPositioner : MonoBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private BoxCollider spawnArea;

    [Header("Books")]
    [SerializeField] private Transform booksParent;
    private Transform[] books;

    [Header("Collision Check")]
    [SerializeField] private Vector3 checkBoxSize = new Vector3(0.35f, 0.15f, 0.25f);
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private int maxAttemptsPerBook = 50;

    [Header("Placement")]
    [SerializeField] private float yPosition = 0.15f;

    private void Start()
    {
        FindBooks();
        RandomizeBooks();
    }

    private void FindBooks()
    {
        books = new Transform[booksParent.childCount];

        for (int i = 0; i < booksParent.childCount; i++)
        {
            books[i] = booksParent.GetChild(i);
        }
    }
    
    public void RandomizeBooks()
    {
        foreach (Transform book in books)
        {
            bool placed = false;

            for (int i = 0; i < maxAttemptsPerBook; i++)
            {
                Vector3 randomPos = GetRandomPointInsideSpawnArea();
                randomPos.y = yPosition;

                Quaternion randomRot = Quaternion.Euler(
                    0f,
                    Random.Range(0f, 360f),
                    0f
                );

                bool blocked = Physics.CheckBox(
                    randomPos,
                    checkBoxSize * 0.5f,
                    randomRot,
                    obstacleMask
                );

                if (!blocked)
                {
                    book.SetPositionAndRotation(randomPos, randomRot);
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                Debug.LogWarning($"Could not find valid position for {book.name}");
            }
        }
    }

    private Vector3 GetRandomPointInsideSpawnArea()
    {
        Bounds bounds = spawnArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, spawnArea.transform.position.y, z);
    }
}