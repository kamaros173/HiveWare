using UnityEngine;
using System.Collections;

public class BigDoor : MonoBehaviour {

    public GameObject[] statues;
    public Sprite sprite;
    public AudioClip openClip;

    private int currentLock;
    private bool locked;
    private SoundManager soundManager;

	// Use this for initialization
	private void Start () {
        currentLock = 0;
        locked = true;
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
	
    public void UnlockDoor(GameObject statue)
    {
        if (locked)
        {
            if (currentLock != 0 && statues[currentLock - 1] == statue)
            {
                //Do Nothing, we want to allow player to hit the statue more than once
                //Play Correct Sound
            }
            else if (statues[currentLock] == statue)
            {
                //Play Correct Sound
                statues[currentLock].SendMessage("Correct");

                currentLock++;
                if (currentLock == statues.Length)
                {
                    StartCoroutine(Unlock());
                }
            }
            else
            {
                statue.SendMessage("Wrong");
                
            }
        }
    }

    private IEnumerator Unlock()
    {

        locked = false;
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<BoxCollider2D>().enabled = false;

        Globals.notFrozen = false;
        Camera.main.SendMessage("Shake", 3f);


        float timer = Time.time + 1.5f;
        while(timer > Time.time)
        {
            yield return null;
        }

        soundManager.PlaySingle(openClip, 0.75f, 0.5f);

        timer += 1.5f;
        while(timer > Time.time)
        {
            yield return null;
        }

        Globals.notFrozen = true;
    }
}
