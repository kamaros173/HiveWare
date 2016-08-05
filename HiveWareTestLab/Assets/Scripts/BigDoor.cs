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
                    locked = false;
                    GetComponent<SpriteRenderer>().sprite = sprite;
                    GetComponent<BoxCollider2D>().enabled = false;
                    soundManager.PlaySingle(openClip, 1f);
                }
            }
            else
            {
                //Play Wrong Sound
                foreach (GameObject s in statues)
                {
                    s.SendMessage("Wrong");
                }
                //currentLock = 0;
            }
        }
    }
}
