using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {

    private bool canAttack = true;

    public float attackSpeedTol;
    public float attackSpeed;

    private void AttackPlayer()
    {
        StartCoroutine(Attack());
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if(other.gameObject.tag == "Player")
    //    {
    //        StartCoroutine(Attack());
    //    }
    //}

    private IEnumerator Attack()
    {
        if (canAttack)
        {
            CircleCollider2D cir2d = gameObject.GetComponent<CircleCollider2D>();
            canAttack = false;
            while (cir2d.radius < attackSpeedTol)
            {
                cir2d.radius = Mathf.Lerp(cir2d.radius, 1.0f, attackSpeed * Time.deltaTime);
                yield return null;
            }
            cir2d.radius = 0.1f;
            canAttack = true;
        }
       
    }
}
