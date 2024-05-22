using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // Movement
    [SerializeField] private float       forwardAcceleration = 10f;
    [SerializeField] private float       maxSpeed            = 10f;

    // Rotation
    [SerializeField] private float       rotationSpeed       = 10f;
    [SerializeField] private float       tiltSpeed           = 10f;
    [SerializeField] private float       tiltAmount          = 15f;
    [SerializeField] private Transform   model;

    // Jumping
    [SerializeField] private float       maxJumpForce        = 10f;
    [SerializeField] private float       minJumpForce        = 5f;
    [SerializeField] private float       jumpChargeRate      = 5f;
    [SerializeField] private float       groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask   groundLayer;

    // Private
    private bool                         isGrounded          = false;
    private float                        currentSpeed        = 0f;
    private Vector3                      velocity            = Vector3.zero;
    private bool                         isChargingJump      = false;
    private float                        currentJumpCharge   = 0f;
    private Rail[]                       rail;
    private bool                         isGrinding          = false;

    // Consts
    private const float                  gravity             = 9.81f;

    private void Awake()
    {
        rail = FindObjectsOfType<Rail>();
        foreach (Rail r in rail)
        {
            r.OnRailGrinding += OnRailGrinding;
            r.OnRailLeaving += OnRailLeaving;
        }
    }

    private void Update() {
        if (isGrinding) return;
        HandleMovement();
        HandleRotation();
        CheckGrounded();
        HandleJump();
        ApplyGravity();
    }

    private float OnRailGrinding()
    {
        transform.GetComponent<CapsuleCollider>().enabled = false;
        isGrinding = true;
        return currentSpeed;
    }

    private void OnRailLeaving() {
        transform.GetComponent<CapsuleCollider>().enabled = true;
        isGrinding = false;
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

    private void HandleMovement() {
        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed += forwardAcceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        } else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, forwardAcceleration * Time.deltaTime);
        }

        transform.position += currentSpeed * Time.deltaTime * transform.forward;
    }

    private void HandleJump() {
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isChargingJump = true;
            } else if (Input.GetKeyUp(KeyCode.Space))
            {
                isChargingJump = false;
                Jump();
            }
        }

        if (isChargingJump) ChargeJump();
    }

    private void ChargeJump() {
        currentJumpCharge += jumpChargeRate * Time.deltaTime;
        currentJumpCharge = Mathf.Clamp(currentJumpCharge, 0f, maxJumpForce);
    }

    private void Jump() {
        velocity = Vector3.zero;
        velocity.y = Mathf.Lerp(minJumpForce, maxJumpForce, currentJumpCharge / maxJumpForce);
        currentJumpCharge = 0f;
        transform.position += velocity * Time.deltaTime;
    }

    private void ApplyGravity() {
        if (!isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
        }
    }

    private void CheckGrounded() {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Obstacle"))
        {
            Vector3 initialForwardPosition = transform.forward;
            Vector3 direction = -initialForwardPosition;
            velocity = direction * 25;
            transform.position += velocity * Time.deltaTime;
        }
    }
}
