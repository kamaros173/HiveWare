using UnityEngine;
using System.Collections;

public class EnemyAttackMage : MonoBehaviour {

    public float attackDelay;
    public GameObject shot;
    public Transform shotSpawn;
    public float beforeAttackDelay;
    public int deathAttackAmount;

    private int deathAttackAngle;

    private void Start()
    {
        deathAttackAngle = 360 / deathAttackAmount;
    }

    private void AttackPlayer(Animator animator)
    {
        StartCoroutine(Attack(animator));
    }

    private IEnumerator Attack(Animator animator)
    {

        float remainingTime = Time.time + beforeAttackDelay;
        while (remainingTime > Time.time)
        {
            yield return null;
        }
        animator.SetTrigger("Attack");

        remainingTime = Time.time + attackDelay;
        while (remainingTime > Time.time)
        {
            yield return null;
        }

        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        gameObject.SendMessageUpwards("EnemyCanNowMove");

    }

    private void DeathAttack()
    {
        for(int i = 0; i < deathAttackAmount; i++)
        {
            Instantiate(shot, shotSpawn.position, Quaternion.Euler(0f, 0f, i * deathAttackAngle));       
        }
        gameObject.SetActive(false);
    }
}
