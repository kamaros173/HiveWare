using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossTrigger : MonoBehaviour {

    public float beforeBossEntranceTimer;
    public float bossEntranceTimer;
    public float afterBossEntranceTimer;
    public GameObject player;
    public GameObject boss;
    public Slider BossHealthBar;
    public GameObject door;
    public SoundManager soundManager;
    public AudioClip bossMusic;
    public AudioClip dungeonMusic;
    private bool activated = false;


    private void PlayerHasBeenHit()
    {
        if (!activated)
        {
            StartCoroutine(Spawn());
            activated = true;
        }
    }

    private IEnumerator Spawn()
    {
        // Pause Game
        Globals.notFrozen = false;
        soundManager.TurnMusicOff();
        soundManager.gameObject.GetComponent<AudioSource>().clip = bossMusic;
        door.SetActive(true);
        float timer = Time.time + beforeBossEntranceTimer;
        while(Time.time < timer)
        {
            yield return null;
        }

        // Boss Enters
        boss.SetActive(true);
        boss.GetComponent<Animator>().enabled = true;
        soundManager.TurnMusicOn();
        boss.SendMessage("EnemyDoneAttacking");
        timer = Time.time + bossEntranceTimer;
        while(Time.time < timer)
        {
            yield return null;
        }

        // Activate Boss Bar
        BossHealthBar.gameObject.SetActive(true);
        boss.SendMessage("EnemyDoneAttacking");
        timer = Time.time + afterBossEntranceTimer;
        while (Time.time < timer)
        {
            yield return null;
        }

        Globals.notFrozen = true;
    }

    private void ResetTrigger()
    {
        activated = false;
        boss.GetComponent<Animator>().enabled = false;
        boss.SetActive(false);
        BossHealthBar.gameObject.SetActive(false);
        door.SetActive(false);
        soundManager.TurnMusicOff();
        soundManager.gameObject.GetComponent<AudioSource>().clip = dungeonMusic;
        soundManager.TurnMusicOn();

    }
}
