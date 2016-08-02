using UnityEngine;
using System.Collections;

public class EnemyAttackBoss : MonoBehaviour {

    public float meleeAttackDistance;
    public float meleeAttackSpeed;
    public float meleeBeforeAttackDelay;
    public float meleeDamageDelay;
    public float meleeAfterAttackDelay;

    public float MagicAttackDelay;
    public GameObject shot;
    public Transform shotSpawn;
    public float MagicBeforeAttackDelay;
    public int MagicAmount;

    public AudioClip SwingClip;
    public AudioClip RangeClip;

    private int deathAttackAngle;
    private Transform player;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        deathAttackAngle = 360 / MagicAmount;
    }

    private void SwingSword(Animator animator)
    {
        StartCoroutine(Swing(animator));
    }

    private IEnumerator Swing(Animator animator)
    {
        CircleCollider2D cir2d = gameObject.GetComponent<CircleCollider2D>();

        float remainingTime = Time.time + meleeBeforeAttackDelay;
        while (remainingTime > Time.time)
        {
            yield return null;
        }
        animator.SetTrigger("Attack");

        remainingTime = Time.time + meleeDamageDelay;
        while (remainingTime > Time.time)
        {
            yield return null;
        }

        cir2d.enabled = true;
        while (cir2d.radius < meleeAttackDistance)
        {
            cir2d.radius += meleeAttackSpeed * Time.deltaTime;
            yield return null;
        }
        cir2d.radius = 0.1f;
        cir2d.enabled = false;

        remainingTime = Time.time + meleeAfterAttackDelay;
        while (remainingTime > Time.time)
        {
            yield return null;
        }
        gameObject.SendMessageUpwards("EnemyDoneAttacking");
    }

    private void CastMagic(Animator animator)
    {
        StartCoroutine(Cast(animator));
    }

    private IEnumerator Cast(Animator animator)
    {

        float remainingTime = Time.time + MagicBeforeAttackDelay;
        while (remainingTime > Time.time)
        {
            yield return null;
        }
        animator.SetTrigger("Magic");

        remainingTime = Time.time + MagicAttackDelay;
        while (remainingTime > Time.time)
        {
            yield return null;
        }

        // Finish Cast
        for (int i = 0; i < MagicAmount; i++)
        {
            Instantiate(shot, shotSpawn.position, Quaternion.Euler(0f, 0f, i * deathAttackAngle));
        }
        gameObject.SendMessageUpwards("EnemyDoneAttacking");

    }

    private void PlayerHasBeenHit()
    {
        GameObject.Find("GameController").SendMessage("HurtPlayer", Vector3.Normalize(player.position - transform.position));
    }
}
