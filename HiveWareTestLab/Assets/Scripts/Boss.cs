using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

    public int maxHealth;
    public float moveSpeed;
    public float meleeRange;
    public float timeBetweenDamage;
    public float timeBetweenAttacks;
    public float timeBetweenRaiseDead;
    public float lowHealthSpeedMuliplier;
    public Color deathColor;
    public LayerMask playerLayer;
    public LayerMask wallLayer;
    public AudioClip hurtClip;
    public AudioClip deathClip;
    public GameObject[] deadEnemies;


    private int currentHealth;
    private float currentMoveSpeed;
    private float currentTimeBetweenAttacks;
    private bool isAttacking;
    private float timeToAttack;
    private float timeToRaiseDead;
    private bool isActive;
    private bool isRaisingDead;
    

    private Transform player;
    private RaycastHit2D raycastToPlayer;
    private Animator animator;
    private SoundManager soundManager;
    private Transform bossAttack;
    private Vector3 startLocation;
    private SpriteRenderer sprite;
    private Color originalColor;
    


    private void Start ()
    {
        currentHealth = maxHealth;
        isAttacking = false;
        currentMoveSpeed = moveSpeed;
        currentTimeBetweenAttacks = timeBetweenAttacks;
        timeToAttack = Time.time + currentTimeBetweenAttacks;
        timeToRaiseDead = Time.time + timeBetweenRaiseDead;
        isActive = false;
        isRaisingDead = false;

        player = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        bossAttack = transform.FindChild("EnemyAttackBoss");
        startLocation = transform.position;
        sprite = transform.GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
    }
	
	private void Update ()
    {
        if (Globals.notFrozen && !isAttacking && isActive)
        {
            if (Time.time > timeToAttack)
                Attack();
            else if (Time.time > timeToRaiseDead && isRaisingDead)
                RaiseDead();
            else
                Chase();
        }
    }

    private void Attack()
    {
        Vector2 dir = Vector3.Normalize(player.position - transform.position);
        raycastToPlayer = Physics2D.Raycast(transform.position, dir, meleeRange, playerLayer);
        isAttacking = true;

        if (raycastToPlayer.collider != null)            
            bossAttack.SendMessage("SwingSword", animator, SendMessageOptions.DontRequireReceiver);
        else
            bossAttack.SendMessage("CastMagic", animator, SendMessageOptions.DontRequireReceiver);
    }

    private void RaiseDead()
    {
        foreach (GameObject enemy in deadEnemies)
        {
            enemy.SetActive(true);
            enemy.SendMessage("BossResurrect");
        }

        timeToRaiseDead = Time.time + timeBetweenRaiseDead;
    }

    private void Chase()
    {
        Vector2 dir = Vector3.Normalize(player.position - transform.position);
        raycastToPlayer = Physics2D.Raycast(transform.position, dir, meleeRange * 0.25f, playerLayer);

        if (raycastToPlayer.collider != null)
        {
            isAttacking = true;
            bossAttack.SendMessage("SwingSword", animator);
        }
        else
        {
            Move(player.position, currentMoveSpeed);
        }
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
        isActive = false;
        isAttacking = false;
        currentHealth = maxHealth;
        transform.position = startLocation;
        isRaisingDead = false;
        currentMoveSpeed = moveSpeed;
        currentTimeBetweenAttacks = timeBetweenAttacks;
        //RESET TRIGGERS
    }

    //CALLED WHEN CAMERA ENTERS ROOM
    private void Load()
    {
        isActive = true;
        timeToAttack = Time.time + currentTimeBetweenAttacks;
        timeToRaiseDead = Time.time + timeBetweenRaiseDead;
    }

    //If the player keeps beating his face against the enmey
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" && Globals.playerIsHittable)
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
        if (other.gameObject.tag == "PlayerSword")
        {
            HitEnemy(Globals.playerSwordDamage);
            if(!isAttacking)
            {
                isAttacking = true;
                bossAttack.SendMessage("SwingSword", animator);
            }
                
        }
        else if (other.gameObject.tag == "Projectile")
        {
            GameObject.Destroy(other.gameObject);
            HitEnemy(Globals.playerArrowDamage);
            if (!isAttacking)
            {
                isAttacking = true;
                bossAttack.SendMessage("CastMagic", animator);
            }
                
        }
        else if (other.gameObject.tag == "MainCamera")
        {
            GameObject.Find("GameController").SendMessage("AddEnemy", transform.gameObject);
            Load();
        }
    }

    private void HitEnemy(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isActive = false;
            GetComponent<SpriteRenderer>().sortingOrder = -1;
            GameObject.Find("GameController").SendMessage("RemoveEnemy", transform.gameObject);

            animator.ResetTrigger("Resurrect");
            animator.SetTrigger("Death");
            soundManager.PlaySingle(deathClip, 1f);
            transform.GetComponent<BoxCollider2D>().enabled = false;

            transform.FindChild("EnemyAttackBoss").gameObject.SetActive(false);
            StartCoroutine(Death());
        }
        else
        {
            StopCoroutine(EnemyDamageFlash());
            sprite.color = originalColor;
            StartCoroutine(EnemyDamageFlash());
            soundManager.PlaySingle(hurtClip, 1f);
            currentTimeBetweenAttacks = timeBetweenAttacks * lowHealthSpeedMuliplier;
            currentMoveSpeed = moveSpeed * lowHealthSpeedMuliplier;


            if(currentHealth / maxHealth < 0.5f)
            {
                isRaisingDead = true;
                timeToRaiseDead = Time.time + timeBetweenRaiseDead;
            }
        }
    }

    private IEnumerator EnemyDamageFlash()
    {
        float waitTime = Time.time + timeBetweenDamage;
        
        Color toColor = Color.red;
        Color fromColor = sprite.color;
        float severity = 0f;

        while (Time.time < waitTime)
        {
            sprite.color = Color.Lerp(fromColor, toColor, severity);
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

        sprite.color = originalColor;
    }

    private void EnemyDoneAttacking()
    {
        isAttacking = false;
        timeToAttack = Time.time + currentTimeBetweenAttacks;
    }

    private IEnumerator Death()
    {
        Vector3 right = new Vector3(transform.position.x + 0.2f, transform.position.y, transform.position.z);
        Vector3 left = new Vector3(transform.position.x - 0.2f, transform.position.y, transform.position.z);
        float timer = Time.time + 7f;
        Vector3 target = right;
        Vector3 nextTarget = left;

        float severity = 0f;

        while (timer > Time.time)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 0.1f);
            sprite.color = Color.Lerp(sprite.color, deathColor, severity * Time.deltaTime);

            if(Vector3.Distance(target, transform.position) < 0.1f)
            {
                severity += 0.005f;
                if(severity > 0.5f)
                {
                    deathColor = new Color(deathColor.r, deathColor.g, deathColor.b, 0f);
                }
                Vector3 temp = target;
                target = nextTarget;
                nextTarget = temp;
            }

            yield return null;
        }

        sprite.color = deathColor;

    }
}
