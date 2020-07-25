using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip HammerSound;
    public AudioClip StepSound;
    public AudioClip Spring;
    public AudioClip DoorSound;
    public static SoundManager instance;
    private void Awake()
    {
        if(SoundManager.instance==null)
        {
            SoundManager.instance = this;
        }
    }

    public void PlayHammerSound()
    {
        audioSource.PlayOneShot(HammerSound);
    }
    public void PlayStepSound()
    {
        audioSource.PlayOneShot(StepSound);
    }
    public void PlaySpringSound()
    {
        audioSource.PlayOneShot(Spring);
    }
    public void PlayDoorSound()
    {
        audioSource.PlayOneShot(DoorSound);
    }


}
