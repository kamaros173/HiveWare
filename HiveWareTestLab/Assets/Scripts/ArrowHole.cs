using UnityEngine;
using System.Collections;

public class ArrowHole : MonoBehaviour {
    
    public float timeBetweenShots;
    public GameObject shot;

    private float nextShot;
	
	// Update is called once per frame
	private void Update () {

        if (nextShot < Time.time)
        {
            nextShot = Time.time + timeBetweenShots;
            Instantiate(shot, transform.position, transform.rotation);
        }
    }
}
