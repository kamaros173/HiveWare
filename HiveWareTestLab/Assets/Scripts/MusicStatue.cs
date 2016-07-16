using UnityEngine;
using System.Collections;

public class MusicStatue : MonoBehaviour {

    public BigDoor door;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerSword" || other.gameObject.tag == "Projectile")
        {
            door.UnlockDoor(gameObject);
        }
    }
}
