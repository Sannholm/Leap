using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudioClipScheduler : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] clips;
    [SerializeField]
    private bool fairScheduling = true;

    private int numUsedClips = 0;

    public void PlayNext()
    {
        int nextIndex = Random.Range(numUsedClips, clips.Length);
        audioSource.PlayOneShot(clips[nextIndex]);

        if (fairScheduling)
        {
            var temp = clips[nextIndex];
            clips[nextIndex] = clips[numUsedClips];
            clips[numUsedClips] = temp;

            numUsedClips = (numUsedClips + 1) % clips.Length;
        }
    }
}
