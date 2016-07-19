using UnityEngine;
using System.Collections;

public class PlayerHitBox : MonoBehaviour {

    private bool playerInHole = false;
    private Player parent; 

    private void Start()
    {
        parent = transform.parent.GetComponent<Player>();
    }
	private void OnTriggerEnter2D(Collider2D other)
    {
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
        else if (other.gameObject.tag == "WallSpike" && Globals.playerIsHittable)
        {
            Globals.notFrozen = false;
            Globals.playerIsHittable = false;
            other.gameObject.SendMessage("PlayerHasBeenHit");
        }
        else if (other.gameObject.tag == "Hole" && !parent.isPlayerDashing() && !playerInHole)
        {
            Globals.notFrozen = false;
            Globals.playerIsHittable = false;
            other.gameObject.SendMessage("PlayerHasBeenHit");
            playerInHole = true;
        }
        else if (other.gameObject.tag == "Trigger")
        {
            other.gameObject.SendMessage("PlayerHasBeenHit");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Hole" && !parent.isPlayerDashing() && !playerInHole)
        {
            Globals.notFrozen = false;
            Globals.playerIsHittable = false;
            other.gameObject.SendMessage("PlayerHasBeenHit");
            playerInHole = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "MainCamera")
        {
            GameObject.Find("Main Camera").SendMessage("MoveCamera", Globals.playerDirection);

        }
    }

    private void Reset()
    {
        playerInHole = false;
    }
}
