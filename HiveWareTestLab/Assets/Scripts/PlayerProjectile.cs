using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour {

    public float speed;
    private Vector3 shotDirection;
    //private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        //rb2d = gameObject.GetComponent<Rigidbody2D>();
        //rb2d.velocity = transform.up * speed;

        if (Globals.playerDirection == PlayerDirection.North)
        {
           
            shotDirection = Vector2.up;
        }
        else if (Globals.playerDirection == PlayerDirection.South)
        {

            shotDirection = Vector2.down;
        }
        else if (Globals.playerDirection == PlayerDirection.East)
        {

            shotDirection = Vector2.down;
        }
        else if (Globals.playerDirection == PlayerDirection.West)
        {

            shotDirection = Vector2.up;
        }
    }

    void Update()
    {
        
        transform.Translate(shotDirection * speed * Time.deltaTime);
    }
}
