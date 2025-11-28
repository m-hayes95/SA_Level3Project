using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth;
    private float health;

    private void Start()
    {
        health = maxHealth;
    }
    
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Debug.Log($"{gameObject.name} was defeated");
            // Play animation and sound
            gameObject.SetActive(false); // Remove
            this.enabled = false;
        }
    }
}
