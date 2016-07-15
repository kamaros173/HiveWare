using UnityEngine;
using System.Collections;

public class WallProjectile : MonoBehaviour {

    public float speed;

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
        //transform.Translate(shotDirection * speed * Time.deltaTime);
    }

    private void PlayerHasBeenHit()
    {
        GameObject.Find("GameController").SendMessage("HurtPlayer", Vector3.Normalize(GameObject.Find("Player").transform.position - transform.position));
        Destroy(gameObject);
    }
}
