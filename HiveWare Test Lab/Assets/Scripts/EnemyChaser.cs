using UnityEngine;
using System.Collections;

public class EnemyChaser : MonoBehaviour {

    public float moveSpeed;
    public float chaseSpeed;
    public float rotateSpeed;
    public float hitRange;
    public LayerMask playerLayer;
    public Vector3[] patrolPoints;

    private Transform player;
    private Vector3 lastPatrolPosition;
    private enum Mode { patrolling, chasing, returning, off};
    private Mode currentState = Mode.patrolling;
    private int patrolPoint = 0;
    private RaycastHit2D hitPlayer;

	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = patrolPoints[patrolPoint];
	}


    void Update ()
    {
        if (Globals.notFrozen)
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
        Vector2 dir = player.position - transform.position;
        hitPlayer = Physics2D.Raycast(transform.position, dir/dir.magnitude, hitRange, playerLayer);
        Debug.DrawRay(transform.position, dir / dir.magnitude);
        if (hitPlayer.collider != null /*Vector3.Distance(transform.position, player.position) < hitRange*/) 
        {
            Debug.Log("ATTACK PLAYER!!");
        }
        else
        {
            Debug.Log("MOVE");
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
        if(transform.position == patrolPoints[patrolPoint])
        {
            if(++patrolPoint == patrolPoints.Length)
            {
                patrolPoint = 0;
            }
        }

        Move(patrolPoints[patrolPoint], moveSpeed);
    }

    private void Move(Vector3 target, float speed)
    {
        Vector3 vectorToTarget = target - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, rotateSpeed * Time.deltaTime);

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    private void Reset()
    {
        //CAN ALSO REVIVE ENEMIES IF DESIRED
        currentState = Mode.off;
        transform.position = patrolPoints[0];
        patrolPoint = 0;
        transform.FindChild("EnemySight").gameObject.SetActive(false);

        Vector3 vectorToTarget = patrolPoints[1] - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = q;
    }

    private void Load()
    {
        currentState = Mode.patrolling;
        transform.FindChild("EnemySight").gameObject.SetActive(true);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player" && Globals.playerIsHittable)
        {
            Debug.Log("IN ENEMY");
            Globals.playerIsHittable = false;
            Globals.notFrozen = false;
            Vector3 contactPoint = other.contacts[0].point;
            Vector3 centerPoint = other.collider.bounds.center;
            Vector3 target = new Vector3(centerPoint.x - contactPoint.x, centerPoint.y - contactPoint.y);
            other.gameObject.SendMessage("HitPlayer", target);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if( other.gameObject.tag == "Player")
        {
            currentState = Mode.chasing;
            lastPatrolPosition = transform.position;
        }
        else if(other.gameObject.tag == "MainCamera")
        {
            GameObject.Find("GameController").SendMessage("AddEnemy" ,transform.gameObject);
            Load();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            currentState = Mode.returning;

    }
}
