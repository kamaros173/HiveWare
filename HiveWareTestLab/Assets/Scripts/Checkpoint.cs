using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

    public Transform checkpoint;
	
    private void PlayerHasBeenHit()
    {
        if(checkpoint.position != transform.position)
        {
            checkpoint.position = transform.position;
            GameObject.Find("GameController").SendMessage("ClearDeadEnemies");
        }
        

        GameObject.Find("GameController").SendMessage("HealPlayer");
    }
	
}
