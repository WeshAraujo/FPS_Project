using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStepsSound : MonoBehaviour
{
    public AudioClip stepClip;
    public AudioSource audioSource;
    // Start is called before the first frame update
   public void PlaySteps()
    {
        audioSource.PlayOneShot(stepClip);
    }
}
