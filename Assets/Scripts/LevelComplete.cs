using UnityEngine;
using UnityEngine.Events;

public class LevelComplete : MonoBehaviour
{
    public UnityEvent OnLevelComplete;
    private bool doOnce = false;
    private void OnTriggerEnter(Collider other)
    {
        if (doOnce) return;
        
        if (other.GetComponent<Player>())
        {
            doOnce = true;
            OnLevelComplete?.Invoke();
        }
    }
}
