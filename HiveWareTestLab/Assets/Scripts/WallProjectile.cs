using UnityEngine;
using System.Collections;

public class WallProjectile : MonoBehaviour {

    public float speed;
    public float WallImmuneTime;

    void Update()
    {
        if (Globals.notFrozen)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }

    private void PlayerHasBeenHit()
    {
        GameObject.Find("GameController").SendMessage("HurtPlayer", Vector3.Normalize(GameObject.Find("Player").transform.position - transform.position));
        Destroy(gameObject);
    }
}
