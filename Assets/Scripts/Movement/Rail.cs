using System;
using System.Collections;
using UnityEngine;

public class Rail : MonoBehaviour
{
    public class MovementData
    {
        public float speed;
        public Vector3 rotation;
        public Func<float, float> handleMovement;
    }

    [System.Serializable]
    public struct Translation
    {
        [SerializeField] public Vector3 Position;
        [SerializeField] public Vector3 Rotation;
    }

    // Public
    [SerializeField] private Translation[] points2;
    [SerializeField] private float         playerRailDistanceThreshold = 0.5f;
    [SerializeField] private GameObject    player;

    // Private
    private bool    playerAttached    = false;
    private int     currentPointIndex = 0;
    private float   playerSpeed       = 0f;
    private bool    disabled          = false;
    private Vector3 firstHitPoint     = Vector3.zero;

    // Events
    public Func<GameObject, MovementData> OnRailGrindingReturnData;
    public Action                         OnRailGrinding;
    public Action                         OnRailLeaving;
    private Func<float, float>            OnHandleMovement;

    private void Update() {
        if (!playerAttached && !disabled) TryAttachPlayerToRail();
        else if (playerAttached) MovePlayerAlongRail();

        if (OnHandleMovement != null)
            playerSpeed = OnHandleMovement.Invoke(playerSpeed);
    }

    public void DetachFromRail()
    {
        disabled = true;
        firstHitPoint = Vector3.zero;
        playerAttached = false;
        OnRailLeaving?.Invoke();
        OnHandleMovement = null;
        StartCoroutine(EnableRail());
    }

    private void TryAttachPlayerToRail() {
        for (int i = 0; i < points2.Length; i++)
        {
            if (i < points2.Length - 1)
            {
                var p1 = transform.position + points2[i].Position;
                var p2 = transform.position + points2[(i + 1)].Position;
                var p3 = player.transform.position;
                var point = ClosestPointOnLineSegment(p1, p2, p3);

                if (IsNearVector(point))
                {
                    var data = OnRailGrindingReturnData?.Invoke(gameObject);
                    OnRailGrinding?.Invoke();
                    playerSpeed = data.speed;
                    OnHandleMovement += data.handleMovement;

                    if (Vector3.Angle(data.rotation, p2 - p1) > 90)
                    {   
                        for (int j = 0; j < points2.Length; j++)
                        {
                            points2[j].Rotation = new Vector3(
                                0, // we dont use this
                                -points2[j].Rotation.y,
                                points2[j].Rotation.z + 180);
                        }

                        Array.Reverse(points2);
                        currentPointIndex = points2.Length - 2 - i;
                    }
                    else
                    {
                        currentPointIndex = i;
                    }

                    playerAttached = true;
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

    private void MovePlayerAlongRail() {
        Vector3 targetPosition = transform.position + points2[currentPointIndex].Position;
        if (firstHitPoint != Vector3.zero) targetPosition = firstHitPoint;

        RotateTowardsPoint();

        Debug.DrawLine(player.transform.position, targetPosition, Color.magenta);

        if (firstHitPoint != Vector3.zero)
            player.transform.position = firstHitPoint;
        else
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, Time.deltaTime * playerSpeed);

        if (Vector3.Distance(player.transform.position, targetPosition) < 0.01f)
        {
            if (firstHitPoint != Vector3.zero) firstHitPoint = Vector3.zero;

            if (currentPointIndex == points2.Length - 1) DetachFromRail();
            else currentPointIndex++;
        }
    }
    private float rotationSpeed = 7f;
    private void RotateTowardsPoint() {

        var rotatePoint = points2[currentPointIndex].Rotation;
        var targetRotation = Quaternion.Euler(rotatePoint.y, rotatePoint.z, rotatePoint.x);

        if (player.transform.localRotation != targetRotation)
        {
            player.transform.localRotation = Quaternion.Slerp(player.transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator EnableRail()
    {
        yield return new WaitForSeconds(.25f);
        disabled = false;
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < points2.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + points2[i].Position, 0.1f);

            if (i < points2.Length - 1)
            {
                var p1 = transform.position + points2[i].Position;
                var p2 = transform.position + points2[(i + 1)].Position;
                var p3 = player.transform.position;

                var point = ClosestPointOnLineSegment(p1, p2, p3);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(point, 0.3f);

                //var endPoint = p1 + (points2[i].Rotation.normalized * 2.0f);
                //Gizmos.color = Color.blue;
                //Gizmos.DrawLine(p1, endPoint);
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
