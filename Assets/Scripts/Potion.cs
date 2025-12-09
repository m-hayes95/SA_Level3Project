using System;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [Range(0.1f,100f)]public float health = 20.0f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.gameObject.name} picked up the potion");
        if (other.gameObject.GetComponent<Health>())
        {
            Health playerHp = other.gameObject.GetComponent<Health>();
            if (playerHp.GetHealth() == playerHp.GetMaxHealth()) return;
            
            playerHp.AddHealth(health);
            gameObject.SetActive(false);
        }
    }
}
