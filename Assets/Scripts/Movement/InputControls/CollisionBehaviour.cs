using UnityEngine;

public class CollisionBehaviour : MonoBehaviour
{
    private int     returnPointDistance = 25;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle")) HandleCollisionDetection();
    }

    private void HandleCollisionDetection()
    {
        Vector3 initialForwardPosition = transform.forward;
        Vector3 direction = -initialForwardPosition;
        transform.position +=direction * returnPointDistance * Time.deltaTime;
    }
}
