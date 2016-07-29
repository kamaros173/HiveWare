using UnityEngine;
using System.Collections;

public class DoorLockTrigger : MonoBehaviour {

    public GameObject[] doors;
    private GameController gc;
    private bool activated;

	// Use this for initialization
	void Start () {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        activated = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!gc.IsThereEnemies() && activated)
        {
            activated = false;
            foreach(GameObject door in doors)
            {
                door.SetActive(false);
            }
        }
	}

    private void PlayerHasBeenHit()
    {
        Debug.Log("PlayerHasBeenHit");
        Debug.Log(gc.IsThereEnemies());
        if (gc.IsThereEnemies())
        {
            activated = true;
            foreach (GameObject door in doors)
            {
               
                door.SetActive(true);
                Debug.Log(door.transform);
            }
        }
    }
}
