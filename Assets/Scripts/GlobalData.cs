using UnityEngine;

public class GlobalData : MonoBehaviour
{
    [SerializeField] private GameObject RailPlayerIsCurrentlyGrindingOn;

    public GameObject GetRailPlayerIsCurrentlyGrindingOn() => RailPlayerIsCurrentlyGrindingOn;
    public void SetRailPlayerIsCurrentlyGrindingOn(GameObject rail) => RailPlayerIsCurrentlyGrindingOn = rail;
}
