using Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class Health : MonoBehaviour, IDamageable
{
    public UnityEvent OnDeath;
    public UnityEvent OnHealthChanged;
    public bool canDamage = true;
    public float maxHealth = 100.0f;
    public Animator animator;
    private const string ISDEAD = "IsDead";
    private const string  HIT = "Hit";
    public AudioSource hitSound;
    [SerializeField] private float hp;
    
    private void Start()
    {
        hp = maxHealth;
    }
    
    public float GetHealth()
    {
        return hp;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
    
    public void AddHealth(float amount)
    {
        hp += amount;
        OnHealthChanged.Invoke();
        if (hp > maxHealth)
        {
            hp = maxHealth;
        }
    }
    
    public void Damage(GameObject instigator, float amount)
    {
        if (!canDamage) return;
        // Don't damage if the player caused the damage
        if (instigator.GetComponent<Player>()) return; 
        animator.SetTrigger(HIT);
        hitSound.Play();
        hp -= amount;
        OnHealthChanged?.Invoke();
        if (hp <= 0)
        {
            Dead();
        }
    }
    
    private void Dead()
    {
        OnDeath.Invoke();
        animator.SetBool(ISDEAD, true);
        this.enabled = false;
        //gameObject.SetActive(false);
        Invoke(nameof(RestartGame), 1f); // change to event
    }
    
    private void RestartGame() // To do, move to different script
    {
        SceneManager.LoadScene(0);
    }
}


