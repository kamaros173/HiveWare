using UnityEngine;
using System.Collections;

public class EnemyAttackChaser : MonoBehaviour {

    private Transform player;

    public float attackDistance;
    public float attackSpeed;
    public float beforeAttackDelay;
    public float damageDelay;
    public float afterAttackDelay;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void AttackPlayer(Animator animator)
    {        
        StartCoroutine(Attack(animator));
    }

    private IEnumerator Attack(Animator animator)
    {       
        CircleCollider2D cir2d = gameObject.GetComponent<CircleCollider2D>();

        float remainingTime = Time.time + beforeAttackDelay;
        while(remainingTime > Time.time)
        {
            yield return null;
        }
        animator.SetTrigger("Attack");

        remainingTime = Time.time + damageDelay;
        while (remainingTime > Time.time)
        {
            yield return null;
        }

        cir2d.enabled = true;
        while (cir2d.radius < attackDistance)
        {
            //cir2d.radius = Mathf.Lerp(cir2d.radius, attackDistance, attackSpeed * Time.deltaTime);
            cir2d.radius += attackSpeed * Time.deltaTime;
            yield return null;
        }
        cir2d.radius = 0.1f;
        cir2d.enabled = false;

        remainingTime = Time.time + afterAttackDelay;
        while (remainingTime > Time.time)
        {
            yield return null;
        }
        gameObject.SendMessageUpwards("EnemyCanNowMove");       
    }

    private void PlayerHasBeenHit()
    {
        GameObject.Find("GameController").SendMessage("HurtPlayer", Vector3.Normalize(player.position - transform.position));
    }
}
