using System;
using UnityEditor.PackageManager;
using UnityEngine;

public class JumpBehaviour : MonoBehaviour
{
    #region Properties

    [SerializeField] private float maxJumpForce = 10f;
    [SerializeField] private float minJumpForce = 5f;
    [SerializeField] private float jumpChargeRate = 5f;
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private LayerMask groundLayer;

    // Private
    private Vector3 velocity = Vector3.zero;
    private bool isChargingJump = false;
    private float currentJumpCharge = 0f;
    private bool isGrounded = false;
    private bool isGrinding = false;
    private Rail[] rail;
    private GlobalData globalData;
    private bool shouldJump = false;

    // Consts
    private const float gravity = 9.81f * 2;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        globalData = FindObjectOfType<GlobalData>();
        if (globalData == null)
        {
            Debug.LogError("GlobalData not found in scene");
        }

        rail = FindObjectsOfType<Rail>();
        foreach (Rail r in rail)
        {
            r.OnRailGrinding += () => isGrinding = true;
        }
    }

    private void FixedUpdate()
    {
        if (!isGrinding)
        {
            isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundLayer);
            HandleGravity();
        }

        Jump();
        ApplyVelocity();
    }

    #endregion

    #region Logical Methods

    public void HandleJumpInput()
    {
        if (isGrounded || isGrinding)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isChargingJump = true;
            } else if (Input.GetKeyUp(KeyCode.Space))
            {
                isChargingJump = false;
                shouldJump = true;
            }
        }

        if (isChargingJump)
        {
            ChargeJump();
        }
    }

    private void ChargeJump()
    {
        currentJumpCharge += jumpChargeRate * Time.deltaTime;
        currentJumpCharge = Mathf.Clamp(currentJumpCharge, 0f, maxJumpForce);
    }

    private void Jump()
    {
        if (!shouldJump) return;
        if (shouldJump) shouldJump = false;

        velocity.y += Mathf.Lerp(minJumpForce, maxJumpForce, currentJumpCharge / maxJumpForce);
        currentJumpCharge = 0f;

        if (isGrinding)
        {
            globalData.GetRailPlayerIsCurrentlyGrindingOn()?.GetComponent<Rail>()?.DetachFromRail();
            isGrinding = false;
        }
    }

    private void HandleGravity()
    {
        if (!isGrounded)
        {
            velocity.y -= gravity * Time.fixedDeltaTime;
        } else if (velocity.y < 0)
        {
            velocity.y = 0;
        }
    }

    private void ApplyVelocity() => transform.position += velocity * Time.fixedDeltaTime;

    #endregion
}
