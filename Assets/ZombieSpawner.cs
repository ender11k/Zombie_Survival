using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform player;
    public float spawnInterval = 3f;
    public float spawnDistance = 12f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnZombie), 1f, spawnInterval);
    }

    void SpawnZombie()
    {
        if (player == null || zombiePrefab == null) return;

        // Pick a random direction at a set radius around the player
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + (Vector3)(randomDir * spawnDistance);

        // Align to integer grid coordinates
        spawnPos.x = Mathf.RoundToInt(spawnPos.x);
        spawnPos.y = Mathf.RoundToInt(spawnPos.y);
        spawnPos.z = 0f;

        GameObject newZombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
        
        // Pass player reference to the newly spawned zombie AI
        ZombieAI ai = newZombie.GetComponent<ZombieAI>();
        if (ai != null)
        {
            ai.player = player;
        }
    }
}
