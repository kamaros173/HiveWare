using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float moveSpeed;
    public float dashDistance;
    public float dashSmooth;
    public float dashTol;
    public LayerMask wallLayer;
    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    public float pushBackSmooth;
    public float pushBackDistance;
    public float pushBackTol;
    public float timeBetweenDamage;
    public float energyToUseShield;
    public float energyToUseDash;
    public float energyToUseArrow;
    public float energyToUseSword;

  
    private float nextFire;
    private Animator animator;
    private GameObject shieldDirection;
    private GameObject swordPivot;
    private GameController gameController;
    private bool isSwinging = false;
    private bool isShooting = false;
    private bool isShielding = false;
    private bool isDashing = false;
    private bool isWalking = false;

    void Start()
    {
        //Get a component reference to the Player's animator component
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        swordPivot = transform.FindChild("SwordPivot").gameObject;
        shieldDirection = transform.FindChild("ShieldNorth").gameObject;
        animator = GetComponent<Animator>();
        nextFire = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Globals.notFrozen)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D))
            {//SWORD
                //////Should we only allow on press down and also allow to reset on pressdown?
                if (gameController.DrainPlayerEnergy(energyToUseSword) && !isSwinging && !isShooting && !isDashing)
                {
                    isSwinging = true;
                    isShielding = false;
                    isWalking = false;
                    shieldDirection.SetActive(false);
                    animator.SetBool("CanWalk", false);
                    animator.SetBool("IsSwinging", true);
                    animator.SetTrigger("Swing");
   
                    swordPivot.SetActive(true);
                    swordPivot.SendMessage("Swing", Globals.playerDirection);

                }
            }
            else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKey(KeyCode.A)) && Time.time > nextFire)
            {//SHOOT
                //////Should we allow the player to interrupt swing to shoot?
                //////Fix Shoot rotation
                if (gameController.DrainPlayerEnergy(energyToUseArrow) && !isSwinging && !isDashing)
                {
                    isShooting = true;
                    isShielding = false;
                    isWalking = false;
                    shieldDirection.SetActive(false);
                    nextFire = Time.time + fireRate;
                    
                    animator.SetBool("CanWalk", false);
                    animator.SetBool("IsShooting", true);
                    animator.SetTrigger("Shoot");
                    if (Globals.playerDirection == PlayerDirection.North)
                    {
                        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                    }
                    else if (Globals.playerDirection == PlayerDirection.South)
                    {
                        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                    }
                    else if (Globals.playerDirection == PlayerDirection.East)
                    {
                        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                    }
                    else if (Globals.playerDirection == PlayerDirection.West)
                    {
                        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {// DASH
                if (gameController.DrainPlayerEnergy(energyToUseDash) && !isSwinging && !isShooting && !isDashing)
                {
                    isDashing = true;
                    isShielding = false;
                    isWalking = false;
                    shieldDirection.SetActive(false);
                    animator.SetBool("CanWalk", false);
                    animator.SetBool("IsDashing", true);
                    animator.SetTrigger("Dash");
                    StartCoroutine(Dash());
                }
            }
            else if (Input.GetKey(KeyCode.S))
            {// SHIELD
                if (gameController.DrainPlayerEnergy(energyToUseShield) && !isSwinging && !isShooting && !isDashing && !isShielding)
                {
                    isShielding = true;
                    isWalking = false;
                    animator.SetBool("CanWalk", false);
                    animator.SetBool("IsShieldUp", true);
                    animator.SetTrigger("ShieldUp");
                    shieldDirection.SetActive(true);
                }
            }

            //KEYUPS
            //SWORD HANDELED AFTER ANIMATION
            //DASHING HANDELED AFTER ANIMATION
            if (Input.GetKeyUp(KeyCode.S))
            {// SHIELD DOWN
                isShielding = false;
                animator.SetBool("CanWalk", true);
                animator.SetBool("IsShieldUp", false);
                animator.ResetTrigger("ShieldUp");
                shieldDirection.SetActive(false);
            }         
            if (Input.GetKeyUp(KeyCode.A))
            {// STOP SHOOTING
                isShooting = false;
                animator.SetBool("CanWalk", true);
                animator.ResetTrigger("Shoot");
                animator.SetBool("IsShooting", false);
            }
            
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            {// STOP WALKING
                isWalking = false;
                animator.SetBool("IsWalking", false);
            }


            //MOVEMENT DIRECTION
            if (!isDashing)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    Globals.playerDirection = PlayerDirection.North;
                    if (!isSwinging)
                    {
                        GetComponent<SpriteRenderer>().flipX = false;
                        animator.SetFloat("MoveX", 0f);  //moveX = 0;
                        animator.SetFloat("MoveY", 1f);  //moveY = 1;
                        shieldDirection.SetActive(false);
                        shieldDirection = transform.FindChild("ShieldNorth").gameObject;
                        if (isShielding)
                            shieldDirection.SetActive(true);

                        //MOVEMENT
                        if (!isShooting && !isShielding)
                        {
                            if (isWalking == false)
                            {
                                isWalking = true;
                                animator.SetBool("IsWalking", true);
                            }

                            animator.SetTrigger("Walking");
                            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
                        }

                    }                                                                     
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    Globals.playerDirection = PlayerDirection.South;
                    if (!isSwinging)
                    {
                        GetComponent<SpriteRenderer>().flipX = false;
                        animator.SetFloat("MoveX", 0f);  // moveX = 0;
                        animator.SetFloat("MoveY", -1f);  // moveY = -1;
                        shieldDirection.SetActive(false);
                        shieldDirection = transform.FindChild("ShieldSouth").gameObject;
                        if (isShielding)
                            shieldDirection.SetActive(true);

                        //MOVEMENT
                        if (!isShooting && !isShielding)
                        {                         
                            if (isWalking == false)
                            {
                                isWalking = true;
                                animator.SetBool("IsWalking", true);
                            }

                            animator.SetTrigger("Walking");
                            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);                           
                        }
                    }             
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    Globals.playerDirection = PlayerDirection.East;
                    if (!isSwinging)
                    {
                        GetComponent<SpriteRenderer>().flipX = false;
                        animator.SetFloat("MoveX", 1f); //moveX = 1;
                        animator.SetFloat("MoveY", 0f); //moveY = 0;
                        shieldDirection.SetActive(false);
                        shieldDirection = transform.FindChild("ShieldEast").gameObject;
                        if (isShielding)
                            shieldDirection.SetActive(true);

                        //MOVEMENT
                        if (!isShooting && !isShielding)
                        {
                            if (isWalking == false)
                            {
                                isWalking = true;
                                animator.SetBool("IsWalking", true);
                            }

                            animator.SetTrigger("Walking");
                            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
                        }

                    }
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Globals.playerDirection = PlayerDirection.West;
                    if (!isSwinging)
                    {
                        GetComponent<SpriteRenderer>().flipX = true;
                        animator.SetFloat("MoveX", -1f); //moveX = -1;
                        animator.SetFloat("MoveY", 0f); //moveY = 0;
                        shieldDirection.SetActive(false);
                        shieldDirection = transform.FindChild("ShieldWest").gameObject;
                        if (isShielding)
                            shieldDirection.SetActive(true);

                        //MOVEMENT
                        if (!isShooting && !isShielding)
                        {
                            if (isWalking == false)
                            {
                                isWalking = true;
                                animator.SetBool("IsWalking", true);
                            }

                            animator.SetTrigger("Walking");
                            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
                        }
                    }                              
                }

                ////MOVEMENT
                //if (!isShooting && !isShielding && !isSwinging)
                //{
                //    if (Input.GetKey(KeyCode.UpArrow) && Globals.playerDirection == PlayerDirection.North)
                //    {
                //        if (isWalking == false)
                //        {
                //            isWalking = true;
                //            animator.SetBool("IsWalking", true);
                //        }

                //        animator.SetTrigger("Walking");
                //        transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
                //    }
                //    else if (Input.GetKey(KeyCode.DownArrow) && Globals.playerDirection == PlayerDirection.South)
                //    {
                //        if (isWalking == false)
                //        {
                //            isWalking = true;
                //            animator.SetBool("IsWalking", true);
                //        }

                //        animator.SetTrigger("Walking");
                //        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
                //    }
                //    else if (Input.GetKey(KeyCode.RightArrow) && Globals.playerDirection == PlayerDirection.East)
                //    {
                //        if (isWalking == false)
                //        {
                //            isWalking = true;
                //            animator.SetBool("IsWalking", true);
                //        }

                //        animator.SetTrigger("Walking");
                //        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
                //    }
                //    else if (Input.GetKey(KeyCode.LeftArrow) && Globals.playerDirection == PlayerDirection.West)
                //    {
                //        if (isWalking == false)
                //        {
                //            isWalking = true;
                //            animator.SetBool("IsWalking", true);
                //        }

                //        animator.SetTrigger("Walking");
                //        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
                //    }
                //}

            }
        }
        else //IF GLOBALS.NOTFROZEN = FALSE
        {
            isSwinging = false;
            isShooting = false;
            isDashing = false;
            isShielding = false;
            isWalking = false;
            animator.SetBool("IsShieldUp", false);
            animator.SetBool("IsWalking", false);
            animator.ResetTrigger("ShieldUp");
            shieldDirection.SetActive(false);
        }
    }

    private IEnumerator Dash()
    {        
        Vector3 target = Vector3.zero;
        Vector2 dir = Vector3.zero;
        RaycastHit2D hit;
        
        //DETERMINE DIRECTION
        if (Globals.playerDirection == PlayerDirection.North)
        {
            target = transform.position + (Vector3.up * dashDistance);
            dir = Vector2.up;
        }
        else if (Globals.playerDirection == PlayerDirection.South)
        {
            target = transform.position + (Vector3.down * dashDistance);
            dir = Vector2.down;
        }
        else if (Globals.playerDirection == PlayerDirection.East)
        {
            target = transform.position + (Vector3.right * dashDistance);
            dir = Vector2.right;
        }
        else if (Globals.playerDirection == PlayerDirection.West)
        {
            target = transform.position + (Vector3.left * dashDistance);
            dir = Vector2.left;
        }

        //DASH
        float oldDis = 0;
        while ((Mathf.Abs(oldDis - Vector3.Distance(transform.position, target)) > dashTol) && Globals.notFrozen)
        {
            oldDis = Vector3.Distance(transform.position, target);

            hit = Physics2D.Raycast(transform.position, dir, oldDis, wallLayer);
            if (hit.collider != null)
                transform.position = Vector3.Lerp(transform.position, hit.point, dashSmooth * Time.deltaTime);
            else
                transform.position = Vector3.Lerp(transform.position, target, dashSmooth * Time.deltaTime);

            yield return null;
        }       
        animator.SetBool("CanWalk", true);
        animator.ResetTrigger("Dash");
        animator.SetBool("IsDashing", false);
        isDashing = false;


    }

    private void HitPlayer(Vector3 direction)
    {
        StartCoroutine(PlayerIsImmuneToDamage());
        StartCoroutine(PushBackPlayer(direction));       
    }
    
    private IEnumerator PlayerIsImmuneToDamage()
    {
        float waitTime = Time.time + timeBetweenDamage;
        Color toColor = Color.red;
        Color fromColor = Color.white;
        float severity = 0f;
        SpriteRenderer damagedsprite = transform.GetComponent<SpriteRenderer>();
        while (Time.time < waitTime)
        {
            if (!Globals.isPlayerDead)
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
        }

        damagedsprite.color = Color.white;
        Globals.playerIsHittable = true;
    }

    private IEnumerator PushBackPlayer(Vector3 direction)
    {

        Vector3 target = transform.position + (direction * pushBackDistance);
        Vector3 oldPos;
        float oldDis = Vector3.Distance(transform.position, target);
        do
        {
            oldPos = transform.position;

            transform.position = Vector3.Lerp(transform.position, target, pushBackSmooth * Time.deltaTime);
            yield return null;

            oldDis = Vector3.Distance(transform.position, target);

        } while ((oldDis > pushBackTol) && Vector3.Distance(transform.position, oldPos) > pushBackTol);

        GameObject.Find("GameController").SendMessage("UnfreezePlayer");

    }

    private void PlayerDeath()
    {
        animator.SetTrigger("Death");
        animator.SetBool("IsDead", true);
    }

    private void PlayerCanSwing()
    {
        transform.FindChild("SwordPivot").gameObject.SetActive(false);
        isSwinging = false;

        if (Globals.playerDirection == PlayerDirection.North)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            animator.SetFloat("MoveX", 0f);  //moveX = 0;
            animator.SetFloat("MoveY", 1f);  //moveY = 1;

        }
        else if (Globals.playerDirection == PlayerDirection.South)
        {

            GetComponent<SpriteRenderer>().flipX = false;
            animator.SetFloat("MoveX", 0f);  // moveX = 0;
            animator.SetFloat("MoveY", -1f);  // moveY = -1;

        }
        else if (Globals.playerDirection == PlayerDirection.East)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            animator.SetFloat("MoveX", 1f); //moveX = 1;
            animator.SetFloat("MoveY", 0f); //moveY = 0;
        }
        else if (Globals.playerDirection == PlayerDirection.West)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            animator.SetFloat("MoveX", -1f); //moveX = -1;
            animator.SetFloat("MoveY", 0f); //moveY = 0;
        }

        //if (!Input.GetKeyDown(KeyCode.D) && !Input.GetKey(KeyCode.D))
        //{// STOP SWINGING          
        animator.SetBool("CanWalk", true);
        animator.ResetTrigger("Swing");
        animator.SetBool("IsSwinging", false);
        //}
    }

    public bool isPlayerDashing()
    {
        return isDashing;
    }

    public void Reset()
    {
        isSwinging = false;
        isShooting = false;
        isShielding = false;
        isDashing = false;
        isWalking = false;
        animator.ResetTrigger("Death");
        animator.SetBool("IsDead", false);
        animator.SetBool("IsDashing", false);
        animator.SetBool("IsShooting", false);
        animator.SetBool("IsSwinging", false);
        animator.SetBool("IsShieldUp", false);
        animator.SetBool("IsWalking", false);
        animator.SetBool("CanWalk", true);
        transform.FindChild("SwordPivot").gameObject.SetActive(false);
        shieldDirection.SetActive(false);
        Globals.isPlayerDead = false;
        Globals.notFrozen = true;
        Globals.playerIsHittable = true;

    }
}