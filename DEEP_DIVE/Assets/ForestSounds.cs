using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestSounds : MonoBehaviour
{
    public AudioSource forestSounds;
    public AudioSource waveSounds;

    private float timeIncrement = .5f;

    private IEnumerator fadeOutSound(AudioSource audio)
    {
        while (audio.volume >= 0)
        {
            audio.volume -= .05f;
            yield return new WaitForSeconds(timeIncrement);
        }

    }
    private IEnumerator fadeInSound(AudioSource audio)
    {
        audio.Play();
        while (audio.volume <= 1)
        {
            audio.volume += .05f;
            yield return new WaitForSeconds(timeIncrement);
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        StopAllCoroutines();
        StartCoroutine(fadeInSound(forestSounds));
        StartCoroutine(fadeOutSound(waveSounds));
    }
    private void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();
        StartCoroutine(fadeOutSound(forestSounds));
        StartCoroutine(fadeInSound(waveSounds));
    }
}
