using UnityEngine;
using System.Collections;

public class EnemyAttackMage : MonoBehaviour {

    public float attackDelay;
    public GameObject shot;
    public Transform shotSpawn;

    private void AttackPlayer()
    {
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        
        float remainingTime = Time.time + attackDelay;
        while (remainingTime > Time.time)
        {
            yield return null;
        }

        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        gameObject.SendMessageUpwards("EnemyCanNowMove");

    }
}
