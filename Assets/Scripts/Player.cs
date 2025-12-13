using System.Collections;
using System.Numerics;
using Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{

    /** Check List
    1. Set up player input controls 1
    2. Make player move and rotate 1
    3. Add dash and cooldown 1
    4. Add Basic Attack 1
    5. Add Animations 1
    6. Create Bomb with logic 1
    7. Throw bomb with player 1
    8. Upgrade Attack to Combo 0
    9. Refactor 0
    **/
    
    public InputActionAsset inputActionAsset;

    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction throwBombAction;
    
    private Rigidbody rigidbody;
    private Vector2 moveInput;

    public UnityEvent OnAttack;
    public UnityEvent OnInteract;
    
    // Movement
    [SerializeField] private int dashCount = 0; 
    
    public float moveSpeed;
    public float rotateSpeed;
    public float dashPower;
    public float dashCooldown = 5;
    public int dashes = 2;
    private const string ISRUNNING = "IsRunning";
    // Bomb Attack
    public Animator  animator;
    public GameObject bombPrefab;
    public Transform bombSpawn;
    public float spawnRate = 4;
    public float throwStrength;
    public float bombFriction;
    private GameObject heldBomb;
    private bool isHoldingBomb = false;
    
    // Health
    
    
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
        interactAction = inputActionAsset.FindAction("Interact");
        jumpAction = inputActionAsset.FindAction("Dash");
        attackAction = inputActionAsset.FindAction("Attack");
        throwBombAction = inputActionAsset.FindAction("ThrowBomb");
        
        rigidbody = GetComponent<Rigidbody>();
    }
    
    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        
        if (throwBombAction.WasPressedThisFrame() )
        {
            if (isHoldingBomb)
            {
                ThrowBomb();
            }
            else
            {
                HoldBomb();
            }
        }
        if (attackAction.WasPressedThisFrame())
        {
            OnAttack?.Invoke();
        }

        if (interactAction.WasPressedThisFrame())
        {
            OnInteract?.Invoke();
        }
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
            animator.SetBool(ISRUNNING, true);
        }
        else
        {
            animator.SetBool(ISRUNNING, false);
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
    
    private void HoldBomb()
    {
        if (heldBomb) return; // If already holding, return 
        
        heldBomb = Instantiate(bombPrefab, bombSpawn.position, bombSpawn.rotation, bombSpawn);
        isHoldingBomb = true;
        
        Invoke(nameof(ResetBomb), spawnRate);
    }

    private void ThrowBomb()
    {
        heldBomb.gameObject.AddComponent<Rigidbody>();
        Rigidbody bombRb = heldBomb.gameObject.GetComponent<Rigidbody>();
        bombRb.linearDamping = bombFriction;
        heldBomb.transform.SetParent(null);
        
        // Make sure the bomb is facing the correct way before throwing, using the bomb spawn forward direction
        Vector3 throwDirection = bombSpawn.forward; 
        bombRb.AddForce(throwDirection * throwStrength, ForceMode.Impulse);
        
        isHoldingBomb = false; 
    }

    private void ResetBomb()
    {
        heldBomb = null; // Make sure we can spawn a new bomb
    }
    
    
}
