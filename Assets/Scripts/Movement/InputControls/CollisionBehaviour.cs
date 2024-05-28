using UnityEngine;

public class CollisionBehaviour : MonoBehaviour
{
    private int returnPointDistance = 25;
    private GlobalData globalData;

    private void Awake()
    {
        globalData = FindObjectOfType<GlobalData>();
        if (globalData == null) Debug.LogError("GlobalData not found in scene");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            var foundParent = CheckNextParent(other.transform);
            if (foundParent == null)
                Debug.LogError("Railing parent with tag not found!");

            if (NotOnRail(foundParent) && NotOnSlope(foundParent))
                HandleCollisionDetection();
        }
    }

    private bool NotOnSlope(Transform foundParent)
    {
        return globalData.GetSlopePlayerIsCurrentlyUsing() == null ||
                        foundParent.gameObject != globalData.GetSlopePlayerIsCurrentlyUsing();
    }

    private bool NotOnRail(Transform foundParent)
    {
        return globalData.GetRailPlayerIsCurrentlyGrindingOn() == null ||
                        foundParent.gameObject != globalData.GetRailPlayerIsCurrentlyGrindingOn();
    }

    private Transform CheckNextParent(Transform transform)
    {
        Transform parent = transform.parent;
        if (parent == null) return null;

        if (!parent.CompareTag("RailingParent"))
        {
            return CheckNextParent(parent);
        }
        else
        {
            return parent;
        }
    }

    private void HandleCollisionDetection()
    {
        Vector3 initialForwardPosition = transform.forward;
        Vector3 direction = -initialForwardPosition;
        transform.position += returnPointDistance * Time.deltaTime * direction;
    }
}
