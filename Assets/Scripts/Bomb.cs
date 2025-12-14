using System.Collections;
using Interfaces;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Range(1.0f,2.0f)] public float pulseSize = 1.5f;
    public float pulseTime = 0.5f;
    public Material pulseMat;
    public float timer = 3f;
    public float damageRadius = 2f;
    public float damage;
    public LayerMask damageableLayer;
    public GameObject explosionEffect;
    // private
    private Material originalMat;
    private Renderer rend;
    private float pulseTimer = 1f;
    private bool isDone = false;
    private Vector3 originalScale;
    private Vector3 pulseScale;
    

    private void Awake()
    {
        rend =  GetComponent<Renderer>();
    }
    private void Start()
    {
        originalMat = rend.material;
        originalScale = transform.localScale;
        pulseScale = originalScale * pulseSize;
        Invoke(nameof(Explode), timer);
        StartCoroutine(ChangeColours());
        // Play timer sound 
    }

    private IEnumerator ChangeColours()
    {
        while (!isDone)
        {
            yield return new WaitForSeconds(pulseTimer);
            Pulse();
            yield return new WaitForSeconds(pulseTime);
            Pulse();
        }
        yield return null;
    }

    private void Pulse()
    {
        rend.material = rend.material != originalMat ? originalMat : pulseMat;

        transform.localScale = transform.localScale != originalScale? originalScale : pulseScale;
    }

    private void Explode()
    {
        // Play sound
        
        Instantiate(explosionEffect,  transform.position, Quaternion.identity);
        
        isDone = true;
        
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
