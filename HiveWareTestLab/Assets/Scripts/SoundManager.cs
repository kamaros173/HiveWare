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

    public void TurnMusicOn()
    {
        musicSource.PlayDelayed(1f);
    }

    public void TurnMusicOff()
    {
        musicSource.Stop();
    }
}
