using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    // Serialized
    [SerializeField] private Transform target;
    [SerializeField] private float     smoothSpeed = 0.125f;
    [SerializeField] private Vector3   offset;


    private void Awake() {
        if (target == null)
        {
            Debug.LogError("CameraSmoothFollow: Target is not set!");
            return;
        }
    }

    void LateUpdate() {
        if (target == null) return;
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.LookAt(target);
    }
}