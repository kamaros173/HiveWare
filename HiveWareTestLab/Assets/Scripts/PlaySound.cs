using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

    private AudioSource efxSource;
    private float lowPitchRange = 0.90f;
    private float highPitchRange = 1.10f;
    private float defaultPitch = 1f;
    private float defaultVol = 1f;
	
    private void MakeNoise(AudioClip clip)
    {
        efxSource = transform.GetComponent<AudioSource>();
        efxSource.volume = defaultVol;
        efxSource.pitch = defaultPitch;
        efxSource.clip = clip;
        efxSource.Play();
        StartCoroutine(Waiting());
    }

    private void MakeNoiseRandom(AudioClip clip)
    {
        efxSource = transform.GetComponent<AudioSource>();
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        efxSource.volume = defaultVol;
        efxSource.pitch = randomPitch * defaultPitch;
        efxSource.clip = clip;
        efxSource.Play();
        StartCoroutine(Waiting());

    }

    private void SetPitch(float p)
    {
        defaultPitch = p;
    }

    private void SetVol(float vol)
    {
        defaultVol = vol;
    }

    private IEnumerator Waiting()
    {
        while (efxSource.isPlaying)
        {
            yield return null;
        }

        Destroy(gameObject);
    }


}
