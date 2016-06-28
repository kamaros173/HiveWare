using UnityEngine;
using System.Collections;

public class EnemyAttackChaser : MonoBehaviour {

    private bool canAttack = true;
    private Transform player;

    public float attackSpeedTol;
    public float attackSpeed;
    public float attackDelay;

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
            CircleCollider2D cir2d = gameObject.GetComponent<CircleCollider2D>();
            float remainingTime = Time.time + attackDelay;

            while(remainingTime > Time.time)
            {
                yield return null;
            }

            while (cir2d.radius < attackSpeedTol)
            {
                cir2d.radius = Mathf.Lerp(cir2d.radius, 1.0f, attackSpeed * Time.deltaTime);
                yield return null;
            }

            cir2d.radius = 0.1f;
            canAttack = true;
        }
       
    }

    private void PlayerHasBeenHit()
    {
        GameObject.Find("GameController").SendMessage("HurtPlayer", Vector3.Normalize(player.position - transform.position));
    }
}
