using System;
using UnityEngine;

public class Slope : MonoBehaviour
{
    [SerializeField] private Vector3[]  points = new Vector3[4];
    [SerializeField] private GameObject player;

    // privates
    private Vector3 centerPoint;
    private Vector3 xDirection;
    private Vector3 yDirection;
    private Vector3 zDirection;

    private bool isInBounds = false;

    // Events
    public Action<GameObject> OnSlopeEnter;
    public Action             OnSlopeExit;

    private void Awake() => GetPlaneWithDirection(out centerPoint, out xDirection, out yDirection, out zDirection);

    private void OnValidate()
    {
        if (points.Length != 4)
        {
            System.Array.Resize(ref points, 4);
        }
    }

    private void Update()
    {
       if (IsPlayerWithinBounds())
       {
            if (!isInBounds)
            {
                OnSlopeEnter?.Invoke(gameObject);
                isInBounds = true;
            }

            AlignPlayerWithSlope();
       }
       else
       {
            if (isInBounds) OnSlopeExit?.Invoke();
            isInBounds = false;

       }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var point in points) Gizmos.DrawSphere(transform.position + point, 0.1f);

        if (points.Length == 4)
        {
            GetPlaneWithDirection(out var center, out var x, out var y, out var z);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + center, x); // X axis
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + center, y); // Y axis
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + center, z); // Z axis
        }
    }

    private void GetPlaneWithDirection(out Vector3 center, out Vector3 x, out Vector3 y, out Vector3 z)
    {
        center = (points[0] + points[1] + points[2] + points[3]) / 4.0f;

        x = (points[2] - points[0]) + (points[3] - points[1]);
        y = (points[2] - points[3]) + (points[0] - points[1]);
        z = Vector3.Cross(x, y);

        x.Normalize();
        y.Normalize();
        z.Normalize();
    }

    private void AlignPlayerWithSlope()
    {
        player.transform.rotation = Quaternion.FromToRotation(Vector3.up, zDirection);
    }

    private bool IsPlayerWithinBounds()
    {
        var localPlayerPosition = player.transform.position - transform.position;

        return IsPointInTriangle(localPlayerPosition, points[0], points[1], points[2]) ||
               IsPointInTriangle(localPlayerPosition, points[1], points[3], points[2]);
    }

    /// <summary>
    /// barycentric coordinate system.
    /// </summary>
    private bool IsPointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        var v0 = c - a;
        var v1 = b - a;
        var v2 = p - a;

        // Compute dot products
        var dot00 = Vector3.Dot(v0, v0);
        var dot01 = Vector3.Dot(v0, v1);
        var dot02 = Vector3.Dot(v0, v2);
        var dot11 = Vector3.Dot(v1, v1);
        var dot12 = Vector3.Dot(v1, v2);

        // Compute barycentric coordinates
        var invDenom = 1 / ((dot00 * dot11) - (dot01 * dot01));
        var u = ((dot11 * dot02) - (dot01 * dot12)) * invDenom;
        var v = ((dot00 * dot12) - (dot01 * dot02)) * invDenom;

        // Check if point is in triangle
        return (u >= 0) && (v >= 0) && (u + v <= 1);
    }
}
