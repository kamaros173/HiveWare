using UnityEngine;
using System.Collections;

public class PlayerShield : MonoBehaviour {

    public SoundManager soundManager;
    public AudioClip shieldHitClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            Debug.Log("HIT");
            soundManager.RandomizeSfx(shieldHitClip, 1f);
            Destroy(other.gameObject);
        }
    }
}
