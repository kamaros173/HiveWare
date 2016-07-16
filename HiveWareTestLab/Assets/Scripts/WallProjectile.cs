using UnityEngine;
using System.Collections;

public class WallProjectile : MonoBehaviour {

    public float speed;
    public float delay;

    private bool firstUpdate = true;

    void Update()
    {
        if (firstUpdate)
        {
            StartCoroutine(DelayCollider());
        }

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

    private IEnumerator DelayCollider()
    {
        float time = Time.time + delay;

        while(time > Time.time)
        {
            yield return null;
        }

        GetComponent<BoxCollider2D>().enabled = true;

    }
}
