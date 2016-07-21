using UnityEngine;
using System.Collections;

public class EnemyBomber : MonoBehaviour {

    public float moveSpeed;
    public float chaseSpeed;
    public LayerMask wallLayer;
    public LayerMask floorLayer;
    public Vector3[] patrolPoints;
    public float explosionDelay;
    public float playerArrowMultiplyer;
    public bool doNotAddToGC;

    private Transform player;
    private Vector3 lastPatrolPosition;
    private enum Mode { patrolling, chasing, returning, off };
    private Mode currentState = Mode.patrolling;
    private int patrolPoint = 0;
    private bool enemyCanMove = true;
    private RaycastHit2D raycastToPlayer;
    private float currentStunTime;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = patrolPoints[patrolPoint];
        animator = GetComponent<Animator>();
    }

    void Update()
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
        Move(player.position, chaseSpeed);       
    }

    private void ReturningToPatrol()
    {
        Move(lastPatrolPosition, moveSpeed);

        if (transform.position == lastPatrolPosition)
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
        else if (vectorToTarget.x < 0f)
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
        patrolPoint = 0;
        animator.ResetTrigger("Death");
        animator.SetTrigger("Resurrect");
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        transform.localScale = new Vector3(1f, 0.75f, 1f);
        transform.position = patrolPoints[0];
        lastPatrolPosition = patrolPoints[0];
        currentState = Mode.patrolling;
        enemyCanMove = true;
        transform.FindChild("EnemyAttackChaser").gameObject.SetActive(true);
        transform.GetComponent<BoxCollider2D>().enabled = true;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" && currentState != Mode.off)
        {
            StartCoroutine(Explode());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerSword")
        {
            StartCoroutine(Explode());
        }
        else if (other.gameObject.tag == "Projectile")
        {
            GameObject.Destroy(other.gameObject);
            StartCoroutine(Explode());
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
            if (Vector3.Distance(new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), other.transform.position) < 0.5f)
            {
                currentState = Mode.off;
                other.gameObject.SendMessage("EnemyHasBeenHit", transform);
            }
        }
    }

    private IEnumerator Explode()
    {

        enemyCanMove = false;
        float waitTime = Time.time + explosionDelay;
        Color toColor = Color.red;
        Color fromColor = Color.white;
        float severity = 0f;
        SpriteRenderer damagedsprite = transform.GetComponent<SpriteRenderer>();
        transform.FindChild("EnemyAttackChaser").SendMessage("AttackPlayer", animator);
        while (Time.time < waitTime)
        {
                damagedsprite.color = Color.Lerp(fromColor, toColor, severity);
                severity += 0.05f;
                if (severity >= 0.9f)
                {
                    Color temp = fromColor;
                    fromColor = toColor;
                    toColor = temp;
                    severity = 0f;
                }

                yield return null;
        }             
        damagedsprite.color = Color.white;
    }

    private void EnemyCanNowMove()
    {
        animator.ResetTrigger("Resurrect");
        animator.SetTrigger("Death");
        GetComponent<SpriteRenderer>().sortingOrder = -1;
        GameObject.Find("GameController").SendMessage("RemoveEnemy", transform.gameObject);
        GameObject.Find("GameController").SendMessage("AddDeadEnemyToList", transform.gameObject);
        animator.ResetTrigger("Resurrect");
        transform.GetComponent<BoxCollider2D>().enabled = false;
        transform.FindChild("EnemyAttackChaser").gameObject.SetActive(false);
    }

    private void StartChase()
    {
        if (currentState != Mode.off)
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
}
