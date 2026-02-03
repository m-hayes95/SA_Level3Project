using UnityEngine;
using System.Collections;

public class FlamethrowerTrap : MonoBehaviour
{
    public bool isLooping = true;
    public bool isOn = false;
    [Range(0.5f, 5f)] public float timer = 3f;
    public ParticleSystem flamethrower;
    public GameObject damageBox;

    // A little clunky as damages player before the full flame and stops before flame comopletely leaves the screen
    private void Start()
    {
        float rand = Random.Range(0f, 5f);
        StartCoroutine(SwitchTimer(rand));
    }

    private IEnumerator SwitchTimer(float delay)
    {
        yield return new WaitForSeconds(delay); // Random delay

        while (isLooping)
        {
            isOn = !isOn; // Switch the bool

            if (isOn)
            {
                damageBox.SetActive(true);
                flamethrower.Play();
            }
            else
            {
                damageBox.SetActive(false);
                flamethrower.Stop();
            }

            yield return new WaitForSeconds(timer);
        }

        yield return null;
    }

}
