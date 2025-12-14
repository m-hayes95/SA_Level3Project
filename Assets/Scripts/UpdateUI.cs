using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateUI : MonoBehaviour
{
    public Slider healthBar;
    public TextMeshProUGUI enemiesDefeatedText;
    
    private Player player;
    

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }
    // call in events
    public void UpdateHealthBar()
    {
        // divide health by 100 so it works with slider (range 0, 1)
        float adjustedValue = player.GetComponent<Health>().GetHealth() / 100;
        healthBar.value = adjustedValue;
    }
    
    public void UpdateEnemiesDefeatedText()
    {
        enemiesDefeatedText.text = player.GetComponent<EnemyDestroyedCounter>().GetEnemiesDefeated().ToString();
    }
}
