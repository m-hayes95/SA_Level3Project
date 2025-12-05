using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
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
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction throwBombAction;
    
    private Rigidbody rigidbody;
    private Vector2 moveInput;
    
    // Movement
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
    public float attackDamage = 40.0f;
    private float nextAttackTime = 0f;
    
    // Bomb Attack
    public GameObject bombPrefab;
    public Transform bombSpawn;
    public float spawnRate = 4;
    public float throwStrength;
    public float bombFriction;
    private GameObject heldBomb;
    private bool isHoldingBomb = false;
    
    // Health
    public float maxHealth = 100.0f;
    [SerializeField] private float hp;
    
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
        throwBombAction = inputActionAsset.FindAction("ThrowBomb");
        rigidbody = GetComponent<Rigidbody>();
        animator = animationsRef.GetComponent<Animator>();
    }

    private void Start()
    {
        hp = maxHealth;
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

    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        this.enabled = false;
        gameObject.SetActive(false);
        Invoke(nameof(RestartGame), 1.0f);
    }

    private void RestartGame() // To do, move to different script
    {
        SceneManager.LoadScene(0);
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

    private void Attack()
    {
        // To do, make attacks combo - Move to attack script
        // Change animations, its jank right now
        animator.SetTrigger("Attack");
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, damageableLayer);
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log($"{enemy.gameObject.name} was hit");
            if (enemy.gameObject.GetComponent<Enemy>())
            {
                enemy.gameObject.GetComponent<Enemy>().TakeDamage(attackDamage); 
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }
}
