using UnityEngine;
using System.Collections;

public class EnemyProjectile : MonoBehaviour {

    public float speed;
    private Vector3 shotDirection;
    private float shotDistance;
    private Vector3 player;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player").transform.position;
        shotDirection = Vector3.Normalize(player - transform.position);
        shotDistance = Vector3.Distance(player, transform.position);
    }

    void Update()
    {
        float traveled = speed * Time.deltaTime;
        transform.Translate(shotDirection * traveled);
        shotDistance -= traveled;
        if(shotDistance < 0)
        {
            Destroy(gameObject, 0.1f);
        }
    }

    private void PlayerHasBeenHit()
    {
        GameObject.Find("GameController").SendMessage("HurtPlayer", Vector3.Normalize(player - transform.position));
        Destroy(gameObject);
    }
}
