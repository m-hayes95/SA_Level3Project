using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{ 
    /**
    1. Set up player input controls
    2. Make player move and rotate
    3. Add dash and cooldown
    4. Add Attack
    5. Add Animations
    **/
    
    [FormerlySerializedAs("inputActionMap")] public InputActionAsset inputActionAsset;

    private InputAction moveAction;
    private InputAction jumpAction;
    
    private Rigidbody rigidbody;
    private Vector2 moveInput;
    [SerializeField]private int dashCount = 0; 

    public float moveSpeed;
    public float rotateSpeed;
    public float dashPower;
    public float dashCooldown = 5;
    public int dashes = 2;
    
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
        jumpAction = inputActionAsset.FindAction("Jump");
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
        
        if (jumpAction.WasPressedThisFrame() && dashCount < dashes)
        { 
            Dash();
        }
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

    private void Dash()
    {
        // Make sure to adjust Rigidbody Linear Damping to prevent over sliding (3f)
        dashCount++;
        rigidbody.AddForceAtPosition(rigidbody.transform.forward * dashPower, rigidbody.position, ForceMode.Force);
        if (dashCount == dashes)
        {
            Invoke(nameof(ResetDash), dashCooldown); 
            // Otherwise will make dashes reset earlier depending on the frame,
            // but now has the issue of needing to hit 3 first
        }
        
    }

    private void ResetDash()
    {
        Debug.Log("Dash cooldown reset");
        dashCount = 0;
    }
}
