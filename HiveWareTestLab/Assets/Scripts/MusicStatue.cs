using UnityEngine;
using System.Collections;

public class MusicStatue : MonoBehaviour {

    public BigDoor door;
    public Color unactiveColor;

    private SpriteRenderer sprite;
    private GameObject player;

    private void Start()
    {
        sprite = transform.GetComponent<SpriteRenderer>();
        sprite.color = unactiveColor;
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        Vector3 vectorToTarget = player.transform.position - transform.position;
        if ( vectorToTarget.y> 0f)
        {// ABOVE TARGET
            GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        else
        {// BELOW TARGET
            GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
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
