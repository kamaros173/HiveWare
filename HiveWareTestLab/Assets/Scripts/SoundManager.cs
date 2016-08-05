using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public AudioSource musicSource;
    public GameObject soundSource;
    

	public void PlaySingle(AudioClip clip, float pitch, float vol = 1f)
    {
        GameObject temp =  (GameObject)Instantiate(soundSource, transform.position, transform.rotation);
        temp.SendMessage("SetVol", vol);
        temp.SendMessage("SetPitch", pitch);
        temp.SendMessage("MakeNoise", clip);
    }

    public void RandomizeSfx(AudioClip clip, float pitch, float vol = 1f)
    {
        GameObject temp = (GameObject)Instantiate(soundSource, transform.position, transform.rotation);
        temp.SendMessage("SetVol", vol);
        temp.SendMessage("SetPitch", pitch);
        temp.SendMessage("MakeNoiseRandom", clip);
    }

    public void TurnMusicOn(/*AudioClip clip, float vol*/)
    {
        musicSource.Play();
    }

    public void TurnMusicOff()
    {
        musicSource.Stop();
    }

    public IEnumerator TransitionSong(AudioClip clip, float vol)
    {
        float timer = Time.time + 1f;

        while(Time.time < timer)
        {
            yield return null;
            
        }
    }
}
