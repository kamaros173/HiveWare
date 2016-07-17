using UnityEngine;
using System.Collections;

public class WallProjectile : MonoBehaviour {

    public float speed;
    public float delay;
    public LayerMask Shield;

    private bool firstUpdate = true;
    private RaycastHit2D hit;
    private Vector3 shotDirection;

    void Start()
    {         
        shotDirection = Vector3.Normalize(GameObject.Find("Player").transform.position - transform.position);
    }

    void Update()
    {
        if (Globals.notFrozen)
        {
            float traveled = 2.5f * speed * Time.deltaTime;

            hit = Physics2D.Raycast(transform.position, shotDirection, traveled, Shield);
            if (hit.collider != null)
            {
                GetComponent<BoxCollider2D>().enabled = false;
                Destroy(gameObject);
                return;
            }
            else
            {
                transform.Translate(Vector2.down * speed * Time.deltaTime);
            }

        }

        if (firstUpdate)
        {
            firstUpdate = false;
            StartCoroutine(DelayCollider());
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
