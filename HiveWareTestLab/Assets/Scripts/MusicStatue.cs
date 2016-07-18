using UnityEngine;
using System.Collections;

public class MusicStatue : MonoBehaviour {

    public BigDoor door;
    public Color unactiveColor;

    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = transform.GetComponent<SpriteRenderer>();
        sprite.color = unactiveColor;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerSword" || other.gameObject.tag == "Projectile")
        {
            door.UnlockDoor(gameObject);
        }
    }

    private void Correct()
    {
        sprite.color = Color.white;
    }

    private void Wrong()
    {
        sprite.color = unactiveColor;
    }
}
