using System.Collections;
using Interfaces;
using UnityEngine;
using UnityEngine.Audio;

public class SwordAttack : MonoBehaviour
{
    // Sword Attack
    public GameObject attackingSword;
    public GameObject sheathedSword;
    private Animator animator;
    public GameObject animationsRef;
    public Transform attackPoint;
    public float attackRange;
    public LayerMask damageableLayer;
    public float attackCooldown = 0.5f;
    public int maxCombo = 3;
    public float comboResetTime = 1.5f;
    public float comboWindowTime = 0.5f;
    public float attackDamage = 20.0f;
    public AudioSource attackSound;
    
    private float lastAttackTime = 0f;
    private bool queuedAttack = false;
    [SerializeField]private bool isAttacking = false;
    [SerializeField]private int comboStep = 1;

    private void Start()
    {
        animator = animationsRef.GetComponent<Animator>();
    }
    private void Update()
    {
        if (lastAttackTime > 0f && !isAttacking && comboStep > 1)
        {
            float timeSinceLastAttack = Time.time - lastAttackTime;
            if (timeSinceLastAttack >= comboResetTime)
            {
                ResetCombo();
            }
        }
    }
    public void TryAttack()
    {
        // Queue Attacks if already attacking and within combo window time
        if (isAttacking)
        {
            if (Time.time - lastAttackTime < comboWindowTime && comboStep < maxCombo)
            {
                queuedAttack = true;
            }
            return;
        }
        ComboAttack();
    }
    private void ComboAttack()
    {
        // Add one to combo counter if last attack time is before reset time
        if (Time.time - lastAttackTime < comboResetTime && comboStep < maxCombo)
        {
            comboStep++;
        }
        else
        {
            comboStep = 1; // Reset combo
        }
        Attack(comboStep);
    }
    private void Attack(int comboIndex)
    {
        SwitchSwords();
        isAttacking = true;
        lastAttackTime = Time.time;
        attackSound.Play();
        // To do, make attacks combo - Move to attack script

        animator.SetTrigger("Attack"); // TODO Change depending on combo index - animator.SetInteger("ComboStep", comboIndex);
        
        // TODO Change to animation events or call a delay to match with combo animation timing or add delay
        
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, damageableLayer);
        foreach (Collider enemy in hitEnemies)
        {
            IDamageable isDamageable = enemy.GetComponent<IDamageable>();
            Debug.Log($"{enemy.gameObject.name} was hit");
            if (isDamageable != null)
            {
                isDamageable.Damage(this.gameObject,ScaleDamage(comboIndex));
            }
        }
        StartCoroutine(ResetAttackState());
    }

    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(attackCooldown);
        SwitchSwords();
        isAttacking = false;
        //Make sword disappear 
        attackingSword.SetActive(false);
        Debug.Log($"Attack state reset - isAttacking set to false, comboStep: {comboStep}");
        
        // If there is a queued attack already pressed, the bool flag will clear and we will re-enter the combo attack,
        // makes attacking feel smoother, but not needed if spam with timing
        if (queuedAttack)
        {
            Debug.Log($"Attack was queued = {queuedAttack}");
            queuedAttack = false;
            ComboAttack(); 
        }
    }
    private float ScaleDamage(int comboNum)
    {
        float damageComboScale = attackDamage * comboNum; // change to a better scale system
        Debug.Log($"Current Combo = {comboNum}, Combo damage = {damageComboScale}");
        return damageComboScale;
    }
    
    private void ResetCombo()
    {
        Debug.Log($"ResetCombo called - comboStep was {comboStep}, resetting to 1. Time since last attack: {Time.time - lastAttackTime}");
        comboStep = 1;
    }

    private void SwitchSwords()
    {
        attackingSword.SetActive(!attackingSword.activeSelf);
        sheathedSword.SetActive(!sheathedSword.activeSelf);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }
}
