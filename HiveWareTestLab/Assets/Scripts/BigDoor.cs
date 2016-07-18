using UnityEngine;
using System.Collections;

public class BigDoor : MonoBehaviour {

    public GameObject[] statues;

    private int currentLock;
    private bool locked;

	// Use this for initialization
	private void Start () {
        currentLock = 0;
        locked = true;
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
                    //Change sprite
                    //Play Victory Sound

                    locked = false;
                    GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            else
            {
                //Play Wrong Sound
                foreach(GameObject s in statues)
                {
                    s.SendMessage("Wrong");
                }
                currentLock = 0;
            }
        }
    }
}
