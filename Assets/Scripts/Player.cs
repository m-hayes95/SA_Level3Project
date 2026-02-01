using System.Collections;
using System.Numerics;
using Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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
    [Header("Input")]
    public InputActionAsset inputActionAsset;

    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction throwBombAction;
    
    private Vector2 moveInput;
    private CharacterController playerController;

    public UnityEvent OnAttack;
    public UnityEvent OnInteract;
    
    [Header("Movement")]
    [SerializeField] private int dashCount = 0; 
    
    public float moveSpeed;
    public float rotateSpeed;
    public float dashPower;
    public float dashCooldown = 5;
    public float dashTime = 1.0f;
    public int dashes = 2;
    public bool isDashing = false;
    public ParticleSystem dashEffect;
    private const string ISRUNNING = "IsRunning";
    private bool canInput = true;
    
    public AudioSource dashSound;
    
    [Header("Bomb Settings")]
    public Animator  animator;
    public GameObject bombPrefab;
    public Transform bombSpawn;
    public float spawnRate = 4;
    public float throwStrength;
    public float bombFriction;
    private GameObject heldBomb;
    private bool isHoldingBomb = false;
    private const string ISHOLDINGBOMB = "IsHolding";
    private const string THROW = "Throw";
    private readonly int dashStateHash =  Animator.StringToHash("Dash");
    public UnityEvent OnDashEvent;
    
    [Header("Animation")]
    public float animationTransitionTime;
    private float startAnimLayerMask;
    private float throwAnimLayerMask;
    
    
    
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
        
        playerController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        canInput = true;
    }
    
    private void Update()
    {
        if (!canInput) return;
        
        
        
        Move();
        
        if (jumpAction.WasPressedThisFrame() && dashCount < dashes && !isDashing)
        { 
            StartCoroutine(Dash());
        }
        
        if (throwBombAction.WasPressedThisFrame() )
        {
            if (isHoldingBomb)
            {
                ThrowBomb();
            }
            else
            {
                HoldBomb();
                // could put the holding blending layer here and not use an Enumerator
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
    
    private void Move()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        Vector3 moveDir =  new Vector3(moveInput.x, 0, moveInput.y);
        moveDir *= moveSpeed;
        playerController.SimpleMove(moveDir);
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

    private IEnumerator Dash()
    {
        isDashing = true;
        dashCount++;
        Vector3 dashDir = transform.forward;
        float timeElapsed = 0;
        dashSound.Play();
        OnDashEvent?.Invoke();
        animator.SetTrigger(dashStateHash);
        StartCoroutine(TransitionAnimationLayers(1, 1f, 1f));
        while (timeElapsed < dashTime)
        {
            playerController.SimpleMove(dashDir * dashPower);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(TransitionAnimationLayers(1, 0f, .5f));
        if (dashCount == dashes)
        {
            yield return new WaitForSeconds(dashCooldown);
            dashCount = 0; 
        }
        isDashing = false;
    }
    
    private IEnumerator TransitionAnimationLayers(int animationLayer, float targetAnimWeight, float transitionTime)
    {
        // Create new layer in animator and blending mask so this layer only affects the hands
        // Set the weight of the layer to 1 to hold and walk or 0 to remove the layer animation
        float startTime = Time.time;
        
        while (Time.time < startTime + transitionTime)
        {
            float currentLayerWeight = animator.GetLayerWeight(animationLayer);
            float newLayerWeight = 
                Mathf.MoveTowards(currentLayerWeight, targetAnimWeight, Time.deltaTime * animationTransitionTime);
            animator.SetLayerWeight(animationLayer, newLayerWeight);
            Debug.Log($"Biggie Smalls {newLayerWeight}");
            yield return null;
        }
        // Force end position 
        animator.SetLayerWeight(animationLayer, targetAnimWeight);
    }
    
    private void HoldBomb()
    {
        if (heldBomb) return; // If already holding, return 
        
        heldBomb = Instantiate(bombPrefab, bombSpawn.position, bombSpawn.rotation, bombSpawn);
        isHoldingBomb = true;
        animator.SetBool(ISHOLDINGBOMB, true);
        StartCoroutine(TransitionAnimationLayers(1, 1f, 1f));
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
        // Anims
        animator.SetBool(ISHOLDINGBOMB, false);
        animator.SetTrigger(THROW);
        StartCoroutine(TransitionAnimationLayers(1, 0f, .5f));
        isHoldingBomb = false; 
    }

    private void ResetBomb()
    {
        animator.SetBool(ISHOLDINGBOMB, false);
        if(animator.GetLayerWeight(1) != 0f)
            StartCoroutine(TransitionAnimationLayers(1, 0f, 1f));
        heldBomb = null; // Make sure we can spawn a new bomb
    }

    public void SetCanUsePlayerInput(bool allowInput = false)
    {
        canInput = allowInput;
    }
}
