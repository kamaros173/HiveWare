using UnityEngine;
using System.Collections;

public class EnemyChaser : MonoBehaviour {

    public int maxHealth;
    public float moveSpeed;
    public float chaseSpeed;
    public float hitRange;
    public LayerMask playerLayer;
    public LayerMask wallLayer;
    public Vector3[] patrolPoints;
    public float pushBackDistance;
    public float pushBackSmooth;
    public float pushBackTol;
    public float stunTime;
    public float timeBetweenDamage;

    private Transform player;
    private Vector3 lastPatrolPosition;
    private enum Mode { patrolling, chasing, returning, off};
    private Mode currentState = Mode.patrolling;
    private int patrolPoint = 0;
    private bool enemyIsHittable = true;
    private bool enemyCanMove = true;
    private int currentHealth;

    private Animator animator;



    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = patrolPoints[patrolPoint];
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    void Update ()
    {
        if (Globals.notFrozen && enemyCanMove)
        {
            if (currentState == Mode.chasing)
            {
                Chase();
            }
            else if (currentState == Mode.returning)
            {
                ReturningToPatrol();
            }
            else if (currentState == Mode.patrolling)
            {
                Patrol();
            }
        }
    }

    private void Chase()
    {
        Vector2 dir = Vector3.Normalize(player.position - transform.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, hitRange, playerLayer);

        if (hit.collider != null) 
        {
            if(hit.collider.tag == "Player")
            {
                enemyCanMove = false;

                if(currentState != Mode.off)
                {
                    transform.FindChild("EnemyAttackChaser").SendMessage("AttackPlayer");
                    animator.SetTrigger("Attack");
                }
                

            }
        }
        else
        {
            Move(player.position, chaseSpeed);
        }
    }

    private void ReturningToPatrol()
    {
        Move(lastPatrolPosition, moveSpeed);

        if(transform.position == lastPatrolPosition)
        {
            currentState = Mode.patrolling;
        }
    }

    private void Patrol()
    {
        if (transform.position == patrolPoints[patrolPoint])
        {
            if (++patrolPoint == patrolPoints.Length)
            {
                patrolPoint = 0;
            }
        }

        Move(patrolPoints[patrolPoint], moveSpeed);
    }

    private void Move(Vector3 target, float speed)
    {
        Vector3 vectorToTarget = target - transform.position;

        //THIS FLIPS DEPENDING ON TARGET DIRECTION
        if(vectorToTarget.x > 0f)
        {
            //LOOK RIGHT
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if(vectorToTarget.x < 0f)
        {
            //LOOK LEFT
            GetComponent<SpriteRenderer>().flipX = true;
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    //CALLED WHEN CAMERA MOVES AWAY FROM SCENE
    private void Reset()
    {
        currentState = Mode.off;
        transform.position = patrolPoints[0];
        patrolPoint = 0;
        transform.FindChild("EnemySightChaser").gameObject.SetActive(false);
        Move(patrolPoints[patrolPoint], moveSpeed);
    }

    //CALLED WHEN CAMERA ENTERS ROOM
    private void Load()
    {
        currentState = Mode.patrolling;
        transform.FindChild("EnemySightChaser").gameObject.SetActive(true);
    }

    //If the player keeps beating his face against the enmey
    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player" && Globals.playerIsHittable)
        {
            Globals.notFrozen = false;
            Globals.playerIsHittable = false;
            Vector3 contactPoint = other.contacts[0].point;
            Vector3 centerPoint = other.collider.bounds.center;
            Vector3 direction = Vector3.Normalize(centerPoint - contactPoint);
            GameObject.Find("GameController").SendMessage("HurtPlayer", direction);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerSword" && enemyIsHittable)
        {
            Vector3 direction = Vector3.Normalize(transform.position - player.position);
            HitEnemy(direction, Globals.playerSwordDamage);
        }
        else if (other.gameObject.tag == "Projectile")
        {
            GameObject.Destroy(other.gameObject);
            if (enemyIsHittable)
            {
                Vector3 direction = Vector3.Normalize(transform.position - player.position);
                HitEnemy(direction, Globals.playerArrowDamage);
            }           
            
        }
        else if (other.gameObject.tag == "MainCamera")
        {
            GameObject.Find("GameController").SendMessage("AddEnemy", transform.gameObject);
            Load();
        }
    }

    private void HitEnemy(Vector3 direction, int damage)
    {
        enemyIsHittable = false;
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentState = Mode.off;
            GameObject.Find("GameController").SendMessage("RemoveEnemy", transform.gameObject);
            //gameObject.SetActive(false);
            animator.SetTrigger("Death");
            transform.GetComponent<BoxCollider2D>().enabled = false;
           
        }
        else
        {
            StartCoroutine(PushBackEnemy(direction));
        }       
    }

    
    private IEnumerator PushBackEnemy(Vector3 direction)
    {        
        currentState = Mode.off;
        Vector3 target = transform.position + (direction * pushBackDistance);
        Vector3 oldPos;
        float oldDis = Vector3.Distance(transform.position, target);
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target, oldDis, wallLayer);
        if (hit.collider != null)
        {
            target = hit.point;
        }

        do
        {
            oldPos = transform.position;

            transform.position = Vector3.Lerp(transform.position, target, pushBackSmooth * Time.deltaTime);
            yield return null;

            oldDis = Vector3.Distance(transform.position, target);
        } while ((oldDis > pushBackTol) && Vector3.Distance(transform.position, oldPos) > pushBackTol) ;

        float stun = Time.time + stunTime;

        while (Time.time < stun)
        {
            yield return null;
        }

        currentState = Mode.chasing;
        enemyIsHittable = true;
    }

    private void StartChase()
    {
        if(currentState != Mode.off)
        {
            currentState = Mode.chasing;
            lastPatrolPosition = transform.position;
        }
        
    }

    private void StopChase()
    {
        if (currentState != Mode.off)
        {
            currentState = Mode.returning;
        }
        
    }

    private void EnemyCanNowMove()
    {
        enemyCanMove = true;
    }
}
