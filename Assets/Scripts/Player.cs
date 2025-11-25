using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{ 
    /**
    1. Set up player input controls
    2. Make player move and rotate
    3.
    4.
    5.
    **/
    
    [FormerlySerializedAs("inputActionMap")] public InputActionAsset inputActionAsset;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    
    private Rigidbody rigidbody;
    private Vector2 moveInput;

    public float moveSpeed;
    public float rotateSpeed;
    public float jumpPower;
    
    private void OnEnable()
    {
        inputActionAsset.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActionAsset.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        moveAction = inputActionAsset.FindAction("Move");
        lookAction = inputActionAsset.FindAction("Look");
        jumpAction = inputActionAsset.FindAction("Jump");
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        // jump
        if (jumpAction.WasPressedThisFrame())
        {
            Jump();
        }
    }

    private void Jump()
    {
        rigidbody.AddForceAtPosition(Vector3.up * jumpPower, Vector3.up, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 moveDir =  new Vector3(moveInput.x, 0, moveInput.y);
        rigidbody.MovePosition(rigidbody.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

        if (moveDir != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.fixedDeltaTime);
        }
    }
}
