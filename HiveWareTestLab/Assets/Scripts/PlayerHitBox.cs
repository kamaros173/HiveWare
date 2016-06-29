using UnityEngine;
using System.Collections;

public class PlayerHitBox : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player HitBox has been hit");
        if (other.gameObject.tag == "EnemyAttack" && Globals.playerIsHittable)
        {
            Globals.notFrozen = false;
            Globals.playerIsHittable = false;
            other.gameObject.SendMessage("PlayerHasBeenHit");
        }
        else if (other.gameObject.tag == "Projectile" && Globals.playerIsHittable)
        {

            GameObject.Destroy(other.gameObject);
            if (Globals.playerIsHittable)
            {
                Globals.notFrozen = false;
                Globals.playerIsHittable = false;
                other.gameObject.SendMessage("PlayerHasBeenHit");
            }

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "MainCamera")
        {
            GameObject.Find("Main Camera").SendMessage("MoveCamera", Globals.playerDirection);
        }
    }
}
