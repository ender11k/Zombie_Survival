using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Transform player;
    public BFSPathfinder pathfinder;
    
    private List<Vector2Int> currentPath = new List<Vector2Int>();
    private int pathIndex = 0;
    private Rigidbody2D rb;
    private LineRenderer lineRenderer; // <-- Declaration added here

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>(); // <-- Initialization added here

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (pathfinder == null)
        {
            pathfinder = FindFirstObjectByType<BFSPathfinder>();
        }

        InvokeRepeating(nameof(RecalculatePath), 0f, 0.25f);
    }

    void RecalculatePath()
    {
        if (player == null || pathfinder == null) return;

        Vector2Int start = pathfinder.WorldToGrid(transform.position);
        Vector2Int target = pathfinder.WorldToGrid(player.position);
        
        currentPath = pathfinder.FindPath(start, target);
        pathIndex = 0;

        DrawPath(); // <-- Calls the draw function
    }

    void DrawPath()
    {
        if (lineRenderer == null || currentPath == null) return;

        lineRenderer.positionCount = currentPath.Count;
        for (int i = 0; i < currentPath.Count; i++)
        {
            Vector3 worldPos = pathfinder.GridToWorld(currentPath[i]);
            worldPos.z = -0.1f; // Slightly in front of floor tiles
            lineRenderer.SetPosition(i, worldPos);
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = player.position;

        if (currentPath != null && pathIndex < currentPath.Count)
        {
            targetPosition = pathfinder.GridToWorld(currentPath[pathIndex]);
            if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
            {
                pathIndex++;
            }
        }

        Vector2 moveDirection = (targetPosition - transform.position).normalized;
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

        RotateTowardDirection(moveDirection);
    }

    void RotateTowardDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.MoveRotation(angle - 90f);
        }
    }
}