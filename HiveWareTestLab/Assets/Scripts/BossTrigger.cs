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


    private void PlayerHasBeenHit()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        // Pause Game
        Globals.notFrozen = false;
        float timer = Time.time + beforeBossEntranceTimer;
        while(Time.time < timer)
        {
            yield return null;
        }

        // Boss Enters
        boss.SetActive(true);
        boss.GetComponent<Animator>().enabled = true;
        timer = Time.time + bossEntranceTimer;
        while(Time.time < timer)
        {
            yield return null;
        }

        // Activate Boss Bar
        BossHealthBar.enabled = true;
        timer = Time.time + afterBossEntranceTimer;
        while (Time.time < timer)
        {
            yield return null;
        }

        Globals.notFrozen = true;
    }
}
