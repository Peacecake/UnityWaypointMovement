using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Color PathColor = Color.white;
    public List<Transform> Waypoints = new List<Transform>();

    public Transform Current
    {
        get
        {
            return this.Waypoints[_index];
        }
    }
    public Transform Next
    {
        get
        {
            if (_index < this.Waypoints.Count - 1)
            {
                _index++;
            }
            return this.Waypoints[_index];
        }
    }
    public Transform Previous
    {
        get
        {
            if (_index > 0)
            {
                _index--;
            }
            return this.Waypoints[_index];
        }
    }

    private int _index;

    private void Start()
    {
        _index = 0;
    }

	private void OnDrawGizmos()
    {
        Gizmos.color = this.PathColor;
        GetWaypoints();
        DrawSpheres();
    }

    /// <summary>
    /// Get all transforms in child objects and add the to Waypoints list.
    /// </summary>
    private void GetWaypoints()
    {
        Waypoints.Clear();
        foreach(Transform child in this.transform)
        {
            if (child != this.transform)
            {
                this.Waypoints.Add(child);
            }
        }
    }

    /// <summary>
    /// Draw a sphere around every waypoint and connect them with a white line.
    /// </summary>
    private void DrawSpheres()
    {
        for (int i = 0; i < Waypoints.Count; i++)
        {
            Vector3 position = Waypoints[i].position;
            Gizmos.DrawWireSphere(position, 0.3f);

            if (i > 0)
            {
                Vector3 previous = Waypoints[i - 1].position;
                Gizmos.DrawLine(previous, position);
            }
        }
    }
}
