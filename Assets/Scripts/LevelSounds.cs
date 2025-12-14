using UnityEngine;

public class LevelSounds : MonoBehaviour
{
    public AudioSource explosionSound;
    public AudioSource doorOpenSound;
    
    public void PlayExplosionSound()
    {
        explosionSound.Play();
    }

    public void PlayDoorOpenSound()
    {
        doorOpenSound.Play();
    }
}
