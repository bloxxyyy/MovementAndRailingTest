using UnityEngine;

public class JumpBehaviour : MonoBehaviour {

    #region Properties

    [SerializeField] private float maxJumpForce = 10f;
    [SerializeField] private float minJumpForce = 5f;
    [SerializeField] private float jumpChargeRate = 5f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    // Private
    private Vector3 velocity          = Vector3.zero;
    private bool    isChargingJump    = false;
    private float   currentJumpCharge = 0f;
    private bool    isGrounded        = false;

    // Consts
    private const float gravity = 9.81f;

    #endregion

    #region Public Methods

    public void HandleJumpBehaviour()
    {
        CheckGrounded();
        HandleJump();
        HandleGravity();
    }

    #endregion

    #region Logical Methods

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void HandleJump()
    {
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

        if (isChargingJump)
            ChargeJump();
    }

    private void ChargeJump()
    {
        currentJumpCharge += jumpChargeRate * Time.deltaTime;
        currentJumpCharge = Mathf.Clamp(currentJumpCharge, 0f, maxJumpForce);
    }

    private void Jump()
    {
        velocity = Vector3.zero;
        velocity.y = Mathf.Lerp(minJumpForce, maxJumpForce, currentJumpCharge / maxJumpForce);
        currentJumpCharge = 0f;
        transform.position += velocity * Time.deltaTime;
    }

    private void HandleGravity()
    {
        if (!isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
        }
    }

    #endregion
}
