using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CollectObjectAudio : MonoBehaviour {

    AudioSource audioSource;
    public AudioClip[] releaseClip;
    public AudioClip[] collectClip;
    public float pitchVariance; 

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayRelease()
    {
        audioSource.pitch = Random.Range(1 - pitchVariance, 1 + pitchVariance);
        audioSource.PlayOneShot(releaseClip[Random.Range(0, releaseClip.Length)]);
    }

    public void PlayCollect()
    {
        audioSource.pitch = Random.Range(1 - pitchVariance, 1 + pitchVariance);
        audioSource.PlayOneShot(collectClip[Random.Range(0, collectClip.Length)]);
        StartCoroutine(DestroySelf());
    }

    private IEnumerator DestroySelf ()
    {
        while (audioSource.isPlaying)
            yield return null;
        Destroy(gameObject);
    }
}
