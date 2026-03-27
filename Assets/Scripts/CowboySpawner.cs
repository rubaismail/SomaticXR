using UnityEngine;

public class CowboySpawner : MonoBehaviour
{
    // drag CowboyRIO_Normal here
    public GameObject cowboyPrefab;

    public float spawnDistance = 3f;

    public float spawnInterval = 5f;

    //How far from the edge of the screen to avoid spawning 
    [Range(0f, 0.4f)]
    public float viewportMargin = 0.15f;

    Camera m_Camera;
    float m_Timer;

    void Start()
    {
        m_Camera = Camera.main;

        if (cowboyPrefab == null)
            Debug.LogWarning("CowboySpawner: No cowboy prefab assigned!", this);

        // Spawn one immediately on start
        SpawnCowboy();
    }

    void Update()
    {
        if (spawnInterval <= 0f) return;

        m_Timer += Time.deltaTime;
        if (m_Timer >= spawnInterval)
        {
            m_Timer = 0f;
            SpawnCowboy();
        }
    }


    /// Spawns a CowboyRIO at a random position within the player's current view. 
    public void SpawnCowboy()
    {
        if (cowboyPrefab == null || m_Camera == null) return;

        // Pick a random viewport point, keeping away from the edges
        float minV = viewportMargin;
        float maxV = 1f - viewportMargin;
        float vpX = Random.Range(minV, maxV);
        float vpY = Random.Range(minV, maxV);

        // Convert viewport point to a world-space ray from the camera
        Ray ray = m_Camera.ViewportPointToRay(new Vector3(vpX, vpY, 0f));

        // Place spawn position along that ray at the desired distance
        Vector3 spawnPosition = ray.origin + ray.direction * spawnDistance;

        // Instantiate the cowboy
        GameObject cowboy = Instantiate(cowboyPrefab, spawnPosition, Quaternion.identity);

        // Rotate cowboy to face the player (only on the Y axis so it stays upright)
        Vector3 lookDir = m_Camera.transform.position - spawnPosition;
        lookDir.y = 0f;
        if (lookDir != Vector3.zero)
            cowboy.transform.rotation = Quaternion.LookRotation(lookDir);
    }
}