﻿using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public AudioSource efxSource;
    public AudioSource musicSource;
    public float lowPitchRange = 0.90f;
    public float highPitchRange = 1.10f;

	// Use this for initialization
	void Start () {
	
	}
	
	public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    public void RandomizeSfx(AudioClip clip)
    {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        efxSource.pitch = randomPitch;
        efxSource.clip = clip;
        efxSource.Play();
    }
}
