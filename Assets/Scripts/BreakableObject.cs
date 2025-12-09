using Interfaces;
using UnityEngine;
using UnityEngine.Events;

public class BreakableObject : MonoBehaviour, IDamageable
{
    public bool onlyDamageWithBomb = true;
    public UnityEvent OnDestroyed;

    public void Damage(GameObject instigator, float amount)
    {
        if (onlyDamageWithBomb)
        {
            if (instigator.GetComponent<Bomb>())
            {
                DestroyThisObject();
            }
        }
        else
        {
            DestroyThisObject();
        }
    }

    private void DestroyThisObject()
    {
        OnDestroyed?.Invoke(); // Activate other effects
        gameObject.SetActive(false);
    }
}
