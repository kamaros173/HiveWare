using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("HIT WALL");
        if(other.gameObject.tag == "Projectile")
        {
            GameObject.Destroy(other.gameObject);
        }
    }
}
