using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float speed;
    //private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        //rb2d = gameObject.GetComponent<Rigidbody2D>();
        //rb2d.velocity = transform.up * speed;
	}

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }
}
