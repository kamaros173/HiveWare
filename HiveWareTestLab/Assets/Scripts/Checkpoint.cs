using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

    public Transform checkpoint;
    public SoundManager soundManager;
    public AudioClip dungeonMusic;
    public AudioClip sanctuaryMusic;
	
    private void PlayerHasBeenHit()
    {
        if(checkpoint.position != transform.position)
        {
            checkpoint.position = transform.position;
            GameObject.Find("GameController").SendMessage("ClearDeadEnemies");
        }

        soundManager.TurnMusicOff();
        soundManager.gameObject.GetComponent<AudioSource>().clip = sanctuaryMusic;
        soundManager.TurnMusicOn();
        GameObject.Find("GameController").SendMessage("HealPlayer");
    }

    private void PlayerHasLeft()
    {
        soundManager.TurnMusicOff();
        soundManager.gameObject.GetComponent<AudioSource>().clip = dungeonMusic;
        soundManager.TurnMusicOn();
    }
	
}
