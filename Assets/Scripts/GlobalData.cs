using UnityEngine;

public class GlobalData : MonoBehaviour
{
    [SerializeField] private GameObject RailPlayerIsCurrentlyGrindingOn;
    [SerializeField] private GameObject SlopePlayerIsCurrentlyUsing;

    public GameObject GetRailPlayerIsCurrentlyGrindingOn() => RailPlayerIsCurrentlyGrindingOn;
    public void SetRailPlayerIsCurrentlyGrindingOn(GameObject rail) => RailPlayerIsCurrentlyGrindingOn = rail;
    public GameObject GetSlopePlayerIsCurrentlyUsing() => SlopePlayerIsCurrentlyUsing;
    public void SetSlopePlayerIsCurrentlyUsing(GameObject slope) => SlopePlayerIsCurrentlyUsing = slope;
}
