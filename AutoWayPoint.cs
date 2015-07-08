using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AutoWayPoint : MonoBehaviour
{
    public static AutoWayPoint[] waypoints;
    public List<AutoWayPoint> connected = new List<AutoWayPoint>();
    public static float kLineOfSightCapsuleRadius = 0.25f;

    public static AutoWayPoint FindClosest(Vector3 pos)
    {
        // The closer two vectors, the larger the dot product will be.
        AutoWayPoint closest = new AutoWayPoint();
        float closestDistance = 1000.0f;
        foreach (AutoWayPoint cur in waypoints)
        {
            float distance = Vector3.Distance(cur.transform.position, pos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = cur;
            }
        }

        return closest;
    }

    [ContextMenu("Update Waypoints")]
    public void UpdateWaypoints()
    {
        RebuildWaypointList();
    }

    public void Awake()
    {
        RebuildWaypointList();
    }

    // Draw the waypoint pickable gizmo
    public void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Waypoint.tif");
    }

    // Draw the waypoint lines only when you select one of the waypoints
    public void OnDrawGizmosSelected()
    {
        if (waypoints.Length == 0)
            RebuildWaypointList();
        foreach (AutoWayPoint p in connected)
        {
            if (Physics.Linecast(transform.position, p.transform.position))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, p.transform.position);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, p.transform.position);
            }
        }
    }

    public void RebuildWaypointList()
    {
        AutoWayPoint[] objects = FindObjectsOfType(typeof(AutoWayPoint)) as AutoWayPoint[];
        waypoints = objects;

        foreach (AutoWayPoint point in waypoints)
        {
            point.RecalculateConnectedWaypoints();
        }
    }

    void RecalculateConnectedWaypoints()
    {
        connected = new List<AutoWayPoint>();

        foreach (AutoWayPoint other in waypoints)
        {
            // Don't connect to ourselves
            if (other == this)
                continue;

            // Do we have a clear line of sight?
            if (!Physics.CheckCapsule(transform.position, other.transform.position, kLineOfSightCapsuleRadius))
            {
                connected.Add(other);
            }
        }
    }
}