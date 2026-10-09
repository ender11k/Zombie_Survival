using UnityEngine;

public class WallObstacle : MonoBehaviour
{
    void Start()
    {
        BFSPathfinder pathfinder = FindObjectOfType<BFSPathfinder>();
        if (pathfinder == null) return;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        
        // Calculate wall boundaries in world space
        Vector3 min = col != null ? col.bounds.min : transform.position - transform.localScale / 2f;
        Vector3 max = col != null ? col.bounds.max : transform.position + transform.localScale / 2f;

        // Iterate through all grid tiles overlapping this wall
        for (float x = min.x; x <= max.x; x += 0.5f)
        {
            for (float y = min.y; y <= max.y; y += 0.5f)
            {
                pathfinder.SetTileWalkable(new Vector3(x, y, 0), false);
            }
        }
    }
}