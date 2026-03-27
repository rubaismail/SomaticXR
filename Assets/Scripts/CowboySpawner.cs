using UnityEngine;

public class CowboySpawner : MonoBehaviour
{
    public GameObject cowboyPrefab;
    public float spawnDistance = 3f;
    public float spawnInterval = 5f;

    [Range(0f, 0.4f)]
    public float viewportMargin = 0.15f;

    Camera m_Camera;
    float m_Timer;

    void Start()
    {
        m_Camera = Camera.main;

        if (cowboyPrefab == null)
        {
            Debug.LogWarning("CowboySpawner: No cowboy prefab assigned!", this);
            return;
        }

        if (cowboyPrefab.GetComponent<CowboySpawner>() != null)
        {
            Debug.LogError("CowboySpawner: The assigned cowboyPrefab has CowboySpawner on it. This will cause infinite spawning/crashing.", this);
            return;
        }

        SpawnCowboy();
    }

    void Update()
    {
        if (spawnInterval <= 0f || cowboyPrefab == null || m_Camera == null) return;

        m_Timer += Time.deltaTime;
        if (m_Timer >= spawnInterval)
        {
            m_Timer = 0f;
            SpawnCowboy();
        }
    }

    public void SpawnCowboy()
    {
        if (cowboyPrefab == null || m_Camera == null) return;

        float minV = viewportMargin;
        float maxV = 1f - viewportMargin;

        float vpX = Random.Range(minV, maxV);
        float vpY = Random.Range(minV, maxV);

        Ray ray = m_Camera.ViewportPointToRay(new Vector3(vpX, vpY, 0f));
        Vector3 spawnPosition = ray.origin + ray.direction * spawnDistance;

        GameObject cowboy = Instantiate(cowboyPrefab, spawnPosition, Quaternion.identity);

        Vector3 lookDir = m_Camera.transform.position - spawnPosition;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
            cowboy.transform.rotation = Quaternion.LookRotation(lookDir);
    }
}