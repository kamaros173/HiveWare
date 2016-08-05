using UnityEngine;
using System.Collections;

public class EnemyAttackBomber : MonoBehaviour {

    private Transform player;
    private Animator animator;
    private SoundManager soundmanager;


    public float attackDistance;
    public float attackSpeed;
    public float beforeAttackDelay;
    public float damageDelay;
    public float afterAttackDelay;
    public AudioClip deathClip;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = transform.parent.GetComponent<Animator>();
        soundmanager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    private void AttackPlayer()
    {
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        CircleCollider2D cir2d = gameObject.GetComponent<CircleCollider2D>();

        float remainingTime = Time.time + beforeAttackDelay;
        while (remainingTime > Time.time)
        {
            yield return null;
        }
        animator.SetTrigger("Attack");

        remainingTime = Time.time + damageDelay;
        while (remainingTime > Time.time)
        {
            yield return null;
        }
        soundmanager.PlaySingle(deathClip, 1.25f, 1f);
        animator.SetTrigger("Explode");
        cir2d.enabled = true;
        while (cir2d.radius < attackDistance)
        {
            cir2d.radius += attackSpeed * Time.deltaTime;
            yield return null;
        }
        cir2d.radius = 0.1f;
        cir2d.enabled = false;

        gameObject.SendMessageUpwards("EnemyCanNowMove");
    }

    private void PlayerHasBeenHit()
    {
        GameObject.Find("GameController").SendMessage("HurtPlayer", Vector3.Normalize(player.position - transform.position));
    }
}
