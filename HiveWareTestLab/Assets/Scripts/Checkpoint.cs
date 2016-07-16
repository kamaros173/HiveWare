using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

    public Transform checkpoint;
	
    private void PlayerHasBeenHit()
    {
        checkpoint.position = transform.position;
    }
	
}
