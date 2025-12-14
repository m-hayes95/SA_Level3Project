using UnityEngine;

public class EnemyDestroyedCounter : MonoBehaviour
{
    
    private int enemiesDefeated = 0;
    
    private void Start()
    {
        enemiesDefeated = 0;
    }

    public void AddToCounter()
    {
        enemiesDefeated++;
    }

    public int GetEnemiesDefeated()
    {
        return enemiesDefeated;
    }
}
