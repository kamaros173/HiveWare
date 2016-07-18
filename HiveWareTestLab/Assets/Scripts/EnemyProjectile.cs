using UnityEngine;
using System.Collections;

public class EnemyProjectile : MonoBehaviour {

    public float speed;
    public LayerMask Shield;
    private Vector3 shotDirection;
    private float shotDistance;
    private Vector3 player;
    private RaycastHit2D hit;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player").transform.position;
        shotDirection = Vector3.Normalize(player - transform.position);
        shotDistance = 2f*Vector3.Distance(player, transform.position);
    }

    void Update()
    {
        if (Globals.notFrozen)
        {
            float traveled = speed * Time.deltaTime;

            hit = Physics2D.Raycast(transform.position, shotDirection, traveled, Shield);
            if (hit.collider != null)
            {
                Destroy(gameObject);
                return;
            }

            transform.Translate(shotDirection * traveled);
            shotDistance -= traveled;
            if (shotDistance < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void PlayerHasBeenHit()
    {
        GameObject.Find("GameController").SendMessage("HurtPlayer", Vector3.Normalize(player - transform.position));
        Destroy(gameObject);
    }
}
