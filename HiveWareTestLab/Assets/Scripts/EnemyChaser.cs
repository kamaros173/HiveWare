using UnityEngine;
using System.Collections;

public class EnemyChaser : MonoBehaviour {

    public int maxHealth;
    public float moveSpeed;
    public float chaseSpeed;
    public float hitRange;
    public LayerMask playerLayer;
    public LayerMask wallLayer;
    public LayerMask floorLayer;
    public Vector3[] patrolPoints;
    public float pushBackDistance;
    public float pushBackSmooth;
    public float pushBackTol;
    public float stunTime;
    public float timeBetweenDamage;
    public float playerArrowMultiplyer;
    public bool doNotAddToGC;
    public AudioClip hitClip;
    public AudioClip deathClip;
    

    private Transform player;
    private Vector3 lastPatrolPosition;
    private enum Mode { patrolling, chasing, returning, off};
    private Mode currentState = Mode.off;
    private int patrolPoint = 0;
    private bool enemyIsHittable = true;
    private bool enemyCanMove = true;
    private int currentHealth;
    private RaycastHit2D raycastToPlayer;
    private float currentStunTime;
    private Animator animator;
    private bool isAttacking = false;
    private SoundManager soundmanager;

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = patrolPoints[patrolPoint];
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        soundmanager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    void Update ()
    {
        if (Globals.notFrozen && enemyCanMove && !isAttacking)
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
        raycastToPlayer = Physics2D.Raycast(transform.position, dir, hitRange, playerLayer);

        if (raycastToPlayer.collider != null) 
        {                        
            enemyCanMove = false;
            isAttacking = true;
            transform.FindChild("EnemyAttackChaser").SendMessage("AttackPlayer", animator);
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

        if (vectorToTarget.x > 0f)
        {// LOOK RIGHT
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if(vectorToTarget.x < 0f)
        {// LOOK LEFT
            GetComponent<SpriteRenderer>().flipX = true;
        }

        if (vectorToTarget.y > 0f)
        {// ABOVE TARGET
            GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        else
        {// BELOW TARGET
            GetComponent<SpriteRenderer>().sortingOrder = -1;
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

    private void Resurrect()
    {
        currentHealth = maxHealth;
        patrolPoint = 0;
        animator.ResetTrigger("Death");
        animator.SetTrigger("Resurrect");
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        transform.localScale = new Vector3(1f, 0.75f, 1f);
        transform.position = patrolPoints[0];
        lastPatrolPosition = patrolPoints[0];
        currentState = Mode.patrolling;
        enemyIsHittable = true;
        enemyCanMove = true;
        isAttacking = false;
        transform.FindChild("EnemyAttackChaser").gameObject.SetActive(true);
        transform.GetComponent<BoxCollider2D>().enabled = true;
    }

    //If the player keeps beating his face against the enmey
    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player" && Globals.playerIsHittable && currentState != Mode.off)
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
            currentStunTime = stunTime;
            HitEnemy(direction, Globals.playerSwordDamage);
        }
        else if (other.gameObject.tag == "Projectile")
        {
            GameObject.Destroy(other.gameObject);
            if (enemyIsHittable && playerArrowMultiplyer != 0)
            {
                currentStunTime = 0f;
                HitEnemy(Vector3.zero, (int)((float)Globals.playerArrowDamage * playerArrowMultiplyer));
            }                      
        }
        else if (other.gameObject.tag == "MainCamera" && !doNotAddToGC)
        {
            GameObject.Find("GameController").SendMessage("AddEnemy", transform.gameObject);
            Load();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Hole" && currentState != Mode.off)
        {
            currentState = Mode.off;
            GameObject.Find("GameController").SendMessage("RemoveEnemy", transform.gameObject);
            other.gameObject.SendMessage("EnemyHasBeenHit", transform);
        }
    }

    private void HitEnemy(Vector3 direction, int damage)
    {        
        enemyIsHittable = false;
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentState = Mode.off;
            GetComponent<SpriteRenderer>().sortingOrder = -1;
            GameObject.Find("GameController").SendMessage("RemoveEnemy", transform.gameObject);
            GameObject.Find("GameController").SendMessage("AddDeadEnemyToList", transform.gameObject);

            animator.ResetTrigger("Resurrect");
            animator.SetTrigger("Death");
            soundmanager.PlaySingle(deathClip, 1f);
            transform.GetComponent<BoxCollider2D>().enabled = false;
            
            transform.FindChild("EnemyAttackChaser").gameObject.SetActive(false);         
        }
        else
        {
            currentState = Mode.chasing;
            StartCoroutine(EnemyIsImmuneToDamage());
            StartCoroutine(PushBackEnemy(direction));
            soundmanager.PlaySingle(hitClip, 1.25f);
        }       
    }

    private IEnumerator EnemyIsImmuneToDamage()
    {
        float waitTime = Time.time + timeBetweenDamage;
        SpriteRenderer damagedsprite = transform.GetComponent<SpriteRenderer>();
        Color original = damagedsprite.color;
        Color toColor = Color.red;
        Color fromColor = damagedsprite.color;
        float severity = 0f;
        
        while (Time.time < waitTime)
        {
            //if (currentState != Mode.off)
            //{
                damagedsprite.color = Color.Lerp(fromColor, toColor, severity);
                severity += 0.05f;
                if (severity >= 0.9f)
                {
                    Color temp = fromColor;
                    fromColor = toColor;
                    toColor = temp;
                    severity = 0f;
                }


            //}
            yield return null;
        }

        damagedsprite.color = original;
        enemyIsHittable = true;
    }


    private IEnumerator PushBackEnemy(Vector3 direction)
    {
        enemyCanMove = false;
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
        } while ((oldDis > pushBackTol) && Vector3.Distance(transform.position, oldPos) > pushBackTol && currentState != Mode.off) ;


        float stun = Time.time + currentStunTime;
        while (Time.time < stun)
        {
            yield return null;
        }
        enemyCanMove = true;
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
        isAttacking = false;
    }
}
