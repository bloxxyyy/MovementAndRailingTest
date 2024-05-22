using System;
using System.Collections;
using UnityEngine;

public class Rail : MonoBehaviour
{

    // Public
    [SerializeField] private Vector3[] points;
    [SerializeField] private float playerRailDistanceThreshold = 0.5f;
    [SerializeField] private GameObject player;

    // Private
    private bool playerAttached = false;
    private int currentPointIndex = 0;
    private float speed = 0f;
    private bool disabled = false;

    // Events
    public Func<float> OnRailGrinding; // returns current character speed
    public Action OnRailLeaving;

    private void Update() {
        if (!playerAttached && !disabled) TryAttachPlayerToRail();
        else if (playerAttached) MovePlayerAlongRail();
    }

    private void TryAttachPlayerToRail() {
        for (int i = 0; i < points.Length; i++)
        {
            if (Vector3.Distance(player.transform.position, transform.position + points[i]) < playerRailDistanceThreshold)
            {
                speed = (float)(OnRailGrinding?.Invoke());
                playerAttached = true;
                currentPointIndex = i;
                break;
            }
        }
    }

    void MovePlayerAlongRail() {
        Vector3 targetPosition = transform.position + points[currentPointIndex];
        player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, Time.deltaTime * speed);

        if (Vector3.Distance(player.transform.position, targetPosition) < 0.01f)
        {
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
        yield return new WaitForSeconds(5);
        disabled = false;
    }

    void OnDrawGizmos() {
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + points[i], 0.1f);
        }
    }
}
