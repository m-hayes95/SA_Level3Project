using UnityEngine;

public class PlayerWalkSounds : MonoBehaviour
{
    // needs to be attached to the same object as the animator, or can be a child?
    public AudioClip[] footstepSounds;
    public AudioSource footstepAudioSource;

    public void PlayRandomFootStep()
    {
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        footstepAudioSource.PlayOneShot(clip);
    }
}
