using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [System.Serializable]
    public class Waypoint
    {
        public Transform point; // assign an empty GameObject in the scene
        public float speed = 2f;
    }

    [Header("Waypoints Settings")]
    public List<Waypoint> waypoints = new List<Waypoint>();

    [Header("Movement Settings")]
    public bool loop = true; // if false, will ping-pong back and forth

    private int currentIndex = 0;
    private int direction = 1;

    void Start()
    {
        if (waypoints.Count > 0 && waypoints[0].point != null)
            transform.position = waypoints[0].point.position; // start at first waypoint
    }

    void Update()
    {
        if (waypoints.Count < 2) return;

        Waypoint current = waypoints[currentIndex];
        Waypoint next = waypoints[(currentIndex + direction + waypoints.Count) % waypoints.Count];

        if (current.point == null || next.point == null) return;

        // Move toward the next waypoint
        transform.position = Vector3.MoveTowards(
            transform.position,
            next.point.position,
            next.speed * Time.deltaTime
        );

        // If reached the next waypoint
        if (Vector3.Distance(transform.position, next.point.position) < 0.01f)
        {
            currentIndex += direction;

            // Handle looping or ping-pong
            if (currentIndex >= waypoints.Count || currentIndex < 0)
            {
                if (loop)
                {
                    currentIndex = (currentIndex + waypoints.Count) % waypoints.Count;
                }
                else
                {
                    direction *= -1;
                    currentIndex += direction * 2; // bounce back properly
                }
            }
        }
    }

    // Draws waypoints in Scene view for easy editing
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i].point == null) continue;

            Gizmos.DrawSphere(waypoints[i].point.position, 0.2f);

            if (i < waypoints.Count - 1 && waypoints[i + 1].point != null)
                Gizmos.DrawLine(waypoints[i].point.position, waypoints[i + 1].point.position);
        }

        if (loop && waypoints.Count > 1 && waypoints[0].point != null && waypoints[waypoints.Count - 1].point != null)
        {
            Gizmos.DrawLine(waypoints[waypoints.Count - 1].point.position, waypoints[0].point.position);
        }
    }
}
