using Interfaces;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float timer = 3f;
    public float damageRadius = 2f;
    public float damage;
    public LayerMask damageableLayer;

    private void Start()
    {
        Invoke(nameof(Explode), timer);
        
        // Play timer sound 
        // Play timer animation
    }

    private void Explode()
    {
        // Play sound
        // Activate Effect
        
        Debug.Log("Detonate Bomb");
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, damageRadius, damageableLayer);
        foreach (Collider obj in hitObjects)
        {
            IDamageable isDamageable = obj.GetComponent<IDamageable>();
            Debug.Log($"{obj.gameObject.name} was hit by {gameObject.name}");
            if (isDamageable != null)
            {
                isDamageable.Damage(gameObject,damage);
            }
                
            // Add damage
        }
        gameObject.SetActive(false);
        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
