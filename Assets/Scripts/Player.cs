using UnityEngine;
using UnityEngine.InputSystem;
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
    6. Upgrade Attack to Combo 0
    7. Refactor 0
    **/
    
    public InputActionAsset inputActionAsset;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    
    private Rigidbody rigidbody;
    private Vector2 moveInput;
    
    [SerializeField]private int dashCount = 0; 

    public float moveSpeed;
    public float rotateSpeed;
    public float dashPower;
    public float dashCooldown = 5;
    public int dashes = 2;

    // Sword Attack
    private Animator animator;
    public GameObject animationsRef;
    public Transform attackPoint;
    public float attackRange;
    public LayerMask damageableLayer;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    
    
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
        jumpAction = inputActionAsset.FindAction("Dash");
        attackAction = inputActionAsset.FindAction("Attack");
        rigidbody = GetComponent<Rigidbody>();
        animator = animationsRef.GetComponent<Animator>();
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        if (Time.time >= nextAttackTime)
        {
            if (attackAction.WasPressedThisFrame())
            {
                nextAttackTime = Time.time + 1f / attackRate;
                Attack();
               
            }
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

    private void Attack()
    {
        // To do, make attacks combo - Move to attack script
        // Change animations, its jank right now
        animator.SetTrigger("Attack");
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, damageableLayer);
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log($"{enemy.gameObject.name} was hit");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }
}
