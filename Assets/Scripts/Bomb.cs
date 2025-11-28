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
            Debug.Log($"{obj.gameObject.name} was hit by {gameObject.name}");
            if (obj.gameObject.GetComponent<Enemy>())
            {
                // Change to interface
                obj.gameObject.GetComponent<Enemy>().TakeDamage(damage); 
            }
                
            // Add damage
        }
        gameObject.SetActive(false);
        this.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
