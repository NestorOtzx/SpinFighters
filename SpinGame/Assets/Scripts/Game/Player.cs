using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : ControllableCharacter
{
    private void Update()
    {
        if (IsOwner || GameManager.instance.isSinglePlayer)
        {
            HandleJumpInput();
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded.Value || isGroundedSingle))
        {
            
            StartCharging();
        }

        if (Input.GetKey(KeyCode.Space) && (isGrounded.Value || isGroundedSingle))
        {
            ChargeJumpForce();
        }

        if (Input.GetKeyUp(KeyCode.Space) && (isGrounded.Value || isGroundedSingle))
        {
            ReleaseJumpForce();
        }
    }
}
