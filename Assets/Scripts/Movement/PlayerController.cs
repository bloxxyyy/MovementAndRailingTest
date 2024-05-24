using System;
using System.Collections;
using UnityEngine;
using static Rail;

public class PlayerController : MonoBehaviour {

    #region Properties

    // Movement
    [SerializeField] private float         forwardAcceleration = 10f;
    [SerializeField] private float         maxSpeed            = 10f;

    // Rotation
    [SerializeField] private float         rotationSpeed       = 10f;
    [SerializeField] private float         tiltSpeed           = 10f;
    [SerializeField] private float         tiltAmount          = 15f;
    [SerializeField] private Transform     model;

    // Ajusting
    [SerializeField] private float         keepUpright         = 5f;

    // Extensions
    [SerializeField] private JumpBehaviour jumpBehaviour;

    // Private
    private float                          currentSpeed        = 0f;
    private Rail[]                         rail;
    private bool                           isGrinding          = false;
    private GlobalData                     globalData;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        globalData = FindObjectOfType<GlobalData>();
        if (globalData == null) Debug.LogError("GlobalData not found in scene");

        rail = FindObjectsOfType<Rail>();
        foreach (Rail r in rail)
        {
            r.OnRailGrindingReturnData += OnRailGrinding;
            r.OnRailLeaving += OnRailLeaving;
        }
    }

    private void Update() {
        if (!isGrinding)
        {
            HandleMovement();
            HandleRotation();
            HandleUpright();
        }

        jumpBehaviour.HandleJumpBehaviour();
    }

    #endregion

    #region Rail Events

    private MovementData OnRailGrinding(GameObject other)
    {
        isGrinding = true;
        globalData.SetRailPlayerIsCurrentlyGrindingOn(other);
        return new MovementData { speed = currentSpeed, rotation = transform.forward };
    }

    private void OnRailLeaving()
    {
        isGrinding = false;
        StartCoroutine(AwaitForRailCollision());
    }

    private IEnumerator AwaitForRailCollision()
    {
        yield return new WaitForSeconds(.1f);
        globalData.SetRailPlayerIsCurrentlyGrindingOn(null);
    }

    #endregion

    #region Logical Methods

    private void HandleMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed += forwardAcceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        } 
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, forwardAcceleration * Time.deltaTime);
        }

        transform.position += currentSpeed * Time.deltaTime * transform.forward;
    }

    private void HandleRotation() {
        float sideways = Input.GetAxis("Horizontal");
        float targetTilt = -sideways * tiltAmount;

        float currentTilt = model.localRotation.eulerAngles.z;
        if (currentTilt > 180) currentTilt -= 360;
        float newTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);
        model.localRotation = Quaternion.Euler(0, 0, newTilt);

        float currentYRotation = transform.rotation.eulerAngles.y;
        float targetYRotation = currentYRotation + (sideways * rotationSpeed);

        float newYRotation = Mathf.LerpAngle(currentYRotation, targetYRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, newYRotation, 0);
    }

    private void HandleUpright() // todo use for ramps
    {
        Quaternion rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * keepUpright);
    }

    #endregion
}
