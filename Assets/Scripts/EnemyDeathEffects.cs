using System.Collections;
using UnityEngine;

public class EnemyDeathEffects : MonoBehaviour
{
    private Enemy enemy;
    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }
    public void PlayDeathEffects()
    {
        StartCoroutine(PlayEffects());
    }
    private IEnumerator PlayEffects()
    {
        enemy.animator.SetTrigger(enemy.deathStateHash);
        enemy.deadAudioSource.Play();
        Instantiate(enemy.deathEffect, transform.position, Quaternion.identity);
        
        yield return new WaitForSeconds(2f);
        ChanceToSpawnPotion();
        enemy.gameObject.SetActive(false);
    }

    private void ChanceToSpawnPotion()
    {
        int rand = Random.Range(0, enemy.potionSpawnChance);
        if (rand == 0)
        {
            Instantiate(enemy.potion, transform.position, transform.rotation);
        }
    }
}
