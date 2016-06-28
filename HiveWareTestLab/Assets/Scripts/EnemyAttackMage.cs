using UnityEngine;
using System.Collections;

public class EnemyAttackMage : MonoBehaviour {

    private bool canAttack = true;
    private Transform player;

    public float attackDelay;
    public GameObject shot;
    public Transform shotSpawn;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    private void AttackPlayer()
    {
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        if (canAttack)
        {
            canAttack = false;
            float remainingTime = Time.time + attackDelay;
            while (remainingTime > Time.time)
            {
                yield return null;
            }

            Vector3 dir = Vector3.Normalize(player.position - transform.position);
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);

            canAttack = true;
        }

    }
}
