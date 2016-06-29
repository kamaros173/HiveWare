using UnityEngine;
using System.Collections;

public class PlayerShield : MonoBehaviour {

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            Destroy(other.gameObject);
        }
    }
}
