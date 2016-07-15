using UnityEngine;
using System.Collections;

public class ArrowHole : MonoBehaviour {
    [HideInInspector] public enum wallDirection
    {
        North,
        South,
        East,
        West
    }
    public wallDirection directionToShoot;
    public float timeBetweenShots;
    public GameObject shot;

    private float nextShot;
	
	// Update is called once per frame
	private void Update () {

        if (nextShot < Time.time)
        {
            nextShot = Time.time + timeBetweenShots;
            if (Globals.playerDirection == PlayerDirection.North)
            {
                Instantiate(shot, transform.position, transform.rotation);
            }
            else if (Globals.playerDirection == PlayerDirection.South)
            {
                Instantiate(shot, transform.position, transform.rotation);
            }
            else if (Globals.playerDirection == PlayerDirection.East)
            {
                Instantiate(shot, transform.position, transform.rotation);
            }
            else if (Globals.playerDirection == PlayerDirection.West)
            {
                Instantiate(shot, transform.position, transform.rotation);
            }
        }
    }
}
