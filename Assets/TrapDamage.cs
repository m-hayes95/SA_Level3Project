using Interfaces;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [Range(1f, 100f)] public float damageAmount = 2f;

    private void OnTriggerStay(Collider other)
    {
        IDamageable isDamageable = other.GetComponent<IDamageable>();
        if (isDamageable != null )
        {
            isDamageable.Damage(this.gameObject, damageAmount);
        }

            
    }
}
