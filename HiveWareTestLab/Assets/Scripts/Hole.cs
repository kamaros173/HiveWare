using UnityEngine;
using System.Collections;

public class Hole : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void PlayerHasBeenHit()
    {
        GameObject.Find("GameController").SendMessage("PlayerInHole", transform.position);
    }
}
