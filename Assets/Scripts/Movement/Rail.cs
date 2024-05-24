using System;
using System.Collections;
using UnityEngine;

public class Rail : MonoBehaviour
{
    public class MovementData
    {
        public float speed;
        public float rotation;
    }

    // Public
    [SerializeField] private Vector3[] points;
    [SerializeField] private float playerRailDistanceThreshold = 0.5f;
    [SerializeField] private GameObject player;

    // Private
    private bool playerAttached = false;
    private int currentPointIndex = 0;
    private float playerSpeed = 0f;
    private bool disabled = false;
    private Vector3 firstHitPoint = Vector3.zero;

    // Events
    public Func<MovementData> OnRailGrinding; // returns current character speed
    public Action OnRailLeaving;

    private void Update() {
        if (!playerAttached && !disabled) TryAttachPlayerToRail();
        else if (playerAttached) MovePlayerAlongRail();
    }

    private void TryAttachPlayerToRail() {
        for (int i = 0; i < points.Length; i++)
        {
            if (i < points.Length - 1)
            {
                var p1 = transform.position + points[i];
                var p2 = transform.position + points[(i + 1)];
                var p3 = player.transform.position;
                var point = ClosestPointOnLineSegment(p1, p2, p3);

                if (IsNearVector(point))
                {
                    var data = OnRailGrinding?.Invoke();
                    playerSpeed = data.speed;

                    if (playerSpeed < 2f) playerSpeed = 2f; // safeguard

                    if (Vector3.Angle(player.transform.forward, p2 - p1) > 90)
                        Array.Reverse(points);

                    playerAttached = true;
                    currentPointIndex = i;
                    firstHitPoint = point;
                    break;
                }
            }
        }
    }

    private bool IsNearVector(Vector3 vector)
    {
        return Vector3.Distance(player.transform.position, vector) < playerRailDistanceThreshold;
    }

    void MovePlayerAlongRail() {
        Vector3 targetPosition = transform.position + points[currentPointIndex];
        if (firstHitPoint != Vector3.zero) targetPosition = firstHitPoint;
        player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, Time.deltaTime * playerSpeed);

        if (Vector3.Distance(player.transform.position, targetPosition) < 0.01f)
        {
            if (firstHitPoint != Vector3.zero) firstHitPoint = Vector3.zero;

            if (currentPointIndex == points.Length - 1)
            {
                disabled = true;
                playerAttached = false;
                OnRailLeaving?.Invoke();
                StartCoroutine(EnableRail());
            } else
            {
                currentPointIndex++;
            }
        }
    }

    IEnumerator EnableRail()
    {
        yield return new WaitForSeconds(1);
        disabled = false;
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + points[i], 0.1f);

            if (i < points.Length - 1)
            {
                var p1 = transform.position + points[i];
                var p2 = transform.position + points[(i + 1)];
                var p3 = player.transform.position;

                var point = ClosestPointOnLineSegment(p1, p2, p3);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(point, 0.3f);
            }

        }
    }

    private Vector3 ClosestPointOnLineSegment(Vector3 p1, Vector3 p2, Vector3 p3) {
        Vector3 direction = p2 - p1;
        float length = direction.magnitude; // make sure minus becomes the same as plus
        direction.Normalize(); // value of 1
        float t = Mathf.Clamp01(Vector3.Dot(p3 - p1, direction) / length);
        return p1 + (direction * (t * length));
    }
}
