using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour {

    public float speed;
    //private Vector3 shotDirection;

	// Use this for initialization
	void Start () {
        if (Globals.playerDirection == PlayerDirection.North)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else if (Globals.playerDirection == PlayerDirection.South)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (Globals.playerDirection == PlayerDirection.East)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
        else if (Globals.playerDirection == PlayerDirection.West)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        }
    }

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
        //transform.Translate(shotDirection * speed * Time.deltaTime);
    }
}
