using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float moveSpeed;
    public float dashDistance;
    public float dashSmooth;
    public float dashTol;
    public LayerMask wallLayer;
    public GameObject shot;
    public GameController gameController;
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

    private bool canSwing = true;
    private bool canMove = true;  
    private float nextFire;
    private Animator animator;
    private float moveX;
    private float moveY;
    private bool isWalking = false;
    private GameObject shieldDirection;

    void Start()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Globals.notFrozen)
        {
            //SWORD HAS HIGHEST PRIORITY
            if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D))
            {
                if (canSwing && gameController.DrainPlayerEnergy(energyToUseSword))
                {
                    if (Globals.playerDirection == PlayerDirection.North)
                    {
                        animator.SetTrigger("Swing");
                    }
                    else if (Globals.playerDirection == PlayerDirection.South)
                    {
                        animator.SetTrigger("Swing");
                    }
                    else if (Globals.playerDirection == PlayerDirection.East)
                    {
                        animator.SetTrigger("Swing");
                    }
                    else if (Globals.playerDirection == PlayerDirection.West)
                    {
                        animator.SetTrigger("Swing");
                    }
                    canSwing = false;
                    transform.FindChild("SwordPivot").gameObject.SetActive(true);
                    transform.FindChild("SwordPivot").gameObject.SendMessage("Swing", Globals.playerDirection);

                }
            }
            else if (Input.GetKey(KeyCode.A) && Time.time > nextFire) //SHOOT
            {
                nextFire = Time.time + fireRate;
                if (gameController.DrainPlayerEnergy(energyToUseArrow))
                {


                    if (Globals.playerDirection == PlayerDirection.North)
                    {
                        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                        animator.SetTrigger("Shoot");
                    }
                    else if (Globals.playerDirection == PlayerDirection.South)
                    {
                        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                        animator.SetTrigger("Shoot");
                    }
                    else if (Globals.playerDirection == PlayerDirection.East)
                    {
                        Instantiate(shot, shotSpawn.position, Quaternion.Euler(0f, 0f, 90f));

                        animator.SetTrigger("Shoot");
                    }
                    else if (Globals.playerDirection == PlayerDirection.West)
                    {
                        Instantiate(shot, shotSpawn.position, Quaternion.Euler(0f, 0f, 90f));

                        animator.SetTrigger("Shoot");
                    }
                }
            }
            else if (Input.GetKey(KeyCode.S)) //SHIELD
            {
                if (gameController.DrainPlayerEnergy(energyToUseShield))
                {
                    canMove = false;
                    animator.SetBool("CanWalk", false);
                    animator.SetBool("IsShieldUp", true);
                    animator.SetTrigger("ShieldUp");

                    if (Globals.playerDirection == PlayerDirection.North)
                    {
                        shieldDirection = transform.FindChild("ShieldNorth").gameObject;

                    }
                    else if (Globals.playerDirection == PlayerDirection.South)
                    {
                        shieldDirection = transform.FindChild("ShieldSouth").gameObject;
                    }
                    else if (Globals.playerDirection == PlayerDirection.East)
                    {
                        shieldDirection = transform.FindChild("ShieldEast").gameObject;
                    }
                    else if (Globals.playerDirection == PlayerDirection.West)
                    {
                        shieldDirection = transform.FindChild("ShieldWest").gameObject;
                    }

                    shieldDirection.SetActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                if (gameController.DrainPlayerEnergy(energyToUseDash))
                {
                    animator.SetTrigger("Dash");
                    StartCoroutine(Dash());
                }
            }


            //KEYUPS
            if (Input.GetKeyUp(KeyCode.S)) //SHIELD DOWN
            {
                canMove = true;
                animator.SetBool("CanWalk", true);
                animator.SetBool("IsShieldUp", false);
                animator.ResetTrigger("ShieldUp");
                shieldDirection.SetActive(false);

            }


            //MOVEMENT
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (isWalking == false)
                {                   
                    isWalking = true;
                    animator.SetBool("IsWalking", true);
                }

                if (canMove)
                {                                     
                    Globals.playerDirection = PlayerDirection.North;
                    moveX = 0;
                    moveY = 1;
                    
                    animator.SetTrigger("Walking");
                    animator.SetFloat("MoveX", moveX);
                    animator.SetFloat("MoveY", moveY);
                    transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
                }
                
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                if (isWalking == false)
                {
                    isWalking = true;
                    animator.SetBool("IsWalking", true);
                }

                if (canMove)
                {
                    Globals.playerDirection = PlayerDirection.South;
                    moveX = 0;
                    moveY = -1;

                    animator.SetTrigger("Walking");
                    animator.SetFloat("MoveX", moveX);
                    animator.SetFloat("MoveY", moveY);
                    transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
                }
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                if (isWalking == false)
                {
                    isWalking = true;
                    animator.SetBool("IsWalking", true);
                }

                if (canMove)
                {
                    Globals.playerDirection = PlayerDirection.East;
                    moveX = 1;
                    moveY = 0;

                    GetComponent<SpriteRenderer>().flipX = false;
                    animator.SetTrigger("Walking");
                    animator.SetFloat("MoveX", moveX);
                    animator.SetFloat("MoveY", moveY);
                    transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (isWalking == false)
                {
                    isWalking = true;
                    animator.SetBool("IsWalking", true);
                }

                if (canMove)
                {
                    Globals.playerDirection = PlayerDirection.West;
                    moveX = -1;
                    moveY = 0;

                    GetComponent<SpriteRenderer>().flipX = true;
                    animator.SetTrigger("Walking");
                    animator.SetFloat("MoveX", moveX);
                    animator.SetFloat("MoveY", moveY);
                    transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
                }
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                isWalking = false;
                animator.SetBool("IsWalking", false);           
            }
        }
        else // If Frozen
        {
            canMove = true;
            animator.SetBool("IsShieldUp", false);
            animator.ResetTrigger("ShieldUp");
            shieldDirection.SetActive(false);
        }
    }

    private IEnumerator Dash()
    {
        if (Globals.notFrozen)
        {
            Vector3 target = Vector3.zero;
            RaycastHit2D hit;
            Vector2 dir = Vector3.zero;

            //DETERMINE DIRECTION
            if (Globals.playerDirection == PlayerDirection.North)
            {
                target = transform.position + Vector3.up * dashDistance;
                dir = Vector2.up;
            }
            else if (Globals.playerDirection == PlayerDirection.South)
            {
                target = transform.position + Vector3.down * dashDistance;
                dir = Vector2.down;
            }
            else if (Globals.playerDirection == PlayerDirection.East)
            {
                target = transform.position + Vector3.right * dashDistance;
                dir = Vector2.right;
            }
            else if (Globals.playerDirection == PlayerDirection.West)
            {
                target = transform.position + Vector3.left * dashDistance;
                dir = Vector2.left;
            }

            //DASH
            float oldDis = 0;
            while ((Mathf.Abs(oldDis - Vector3.Distance(transform.position, target)) > dashTol) && Globals.notFrozen)
            {
                oldDis = Vector3.Distance(transform.position, target);

                hit = Physics2D.Raycast(transform.position, dir, oldDis, wallLayer);
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject);
                    transform.position = Vector3.Lerp(transform.position, hit.point, dashSmooth * Time.deltaTime);
                }
                else
                    transform.position = Vector3.Lerp(transform.position, target, dashSmooth * Time.deltaTime);

                yield return null;
            }
        }
    }

    private void HitPlayer(Vector3 direction)
    {
        StartCoroutine(PushBackPlayer(direction));
        StartCoroutine(PlayerIsImmuneToDamage());
    }

    private IEnumerator PlayerIsImmuneToDamage()
    {
        float waitTime = Time.time + timeBetweenDamage;

        while(Time.time < waitTime)
        {
            yield return null;
        }

        Globals.playerIsHittable = true;
    }

    private IEnumerator PushBackPlayer(Vector3 direction)
    {

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
        } while ((oldDis > pushBackTol) && Vector3.Distance(transform.position, oldPos) > pushBackTol);

        Globals.notFrozen = true;
    }

    private void PlayerCanSwing()
    {
        transform.FindChild("SwordPivot").gameObject.SetActive(false);
        canSwing = true;
    }
}



// if (Input.GetKey(KeyCode.S))
// {
//     if (gameController.DrainPlayerEnergy(energyToUseShield))
//     {
//         transform.FindChild("Shield").gameObject.SetActive(true);

//         if (Globals.playerDirection == PlayerDirection.North)
//         {
//             animator.SetBool("PlayerShieldDown", false);
//             animator.SetTrigger("PlayerShieldUp");

//         }
//         else if (Globals.playerDirection == PlayerDirection.South)
//         {
//             animator.SetBool("PlayerShieldDown", false);
//             animator.SetTrigger("PlayerDownShield");
//         }
//         else if (Globals.playerDirection == PlayerDirection.East || Globals.playerDirection == PlayerDirection.West)
//         {
//             animator.SetBool("PlayerShieldDown", false);
//             animator.SetTrigger("PlayerSideShield");
//         }

//     }
// }
// //MOVE
// else if (Input.GetKey(KeyCode.UpArrow))
// {
//     Globals.playerDirection = PlayerDirection.North;
//     transform.rotation = Quaternion.Euler(0f, 0f, 0f);
//     transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
//     animator.SetTrigger("PlayerWalkUp");
// }
// else if (Input.GetKeyUp(KeyCode.UpArrow))
// {
//     animator.SetTrigger("PlayerIdleUp");
// }

// else if (Input.GetKey(KeyCode.DownArrow))
// {
//     Globals.playerDirection = PlayerDirection.South;
//     //transform.rotation = Quaternion.Euler(0f, 0f, 180f);
//     transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
//     animator.ResetTrigger("PlayerIdleUp");
//     animator.SetTrigger("PlayerIdleDown");
//     animator.SetTrigger("PlayerWalkDown");

// }
// else if (Input.GetKeyUp(KeyCode.DownArrow))
// {
//     animator.SetTrigger("PlayerIdleDown");
// }
// else if (Input.GetKey(KeyCode.RightArrow))
// {
//     Globals.playerDirection = PlayerDirection.East;
//     //transform.rotation = Quaternion.Euler(0f, 0f, 270f);
//     GetComponent<SpriteRenderer>().flipX = false;
//     animator.ResetTrigger("PlayerIdleDown");
//     animator.ResetTrigger("PlayerIdleUp");
//     animator.SetTrigger("PlayerWalkSide");
//     transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);

// }
// else if (Input.GetKeyUp(KeyCode.RightArrow))
// {
//     animator.SetTrigger("PlayerIdleSide");
// }

// else if (Input.GetKey(KeyCode.LeftArrow))
// {
//     Globals.playerDirection = PlayerDirection.West;
//     GetComponent<SpriteRenderer>().flipX = true;
//     //transform.rotation = Quaternion.Euler(0f, 0f, 90f);
//     animator.ResetTrigger("PlayerIdleDown");
//     animator.ResetTrigger("PlayerIdleUp");
//     animator.SetTrigger("PlayerWalkSide");
//     transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
// }
// else if (Input.GetKeyUp(KeyCode.LeftArrow))
// {
//     animator.SetTrigger("PlayerIdleSide");
// }


// //SHIELD (CURRENTLY ALLOWING SHIELD, SWORD, AND DASH AT THE SAME TIME)


// //SWORD
// else if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D))
// {
//     if (canSwing && gameController.DrainPlayerEnergy(energyToUseSword))
//     {
//         if (Globals.playerDirection == PlayerDirection.North)
//         {
//             animator.SetTrigger("SwordSwingUP");
//         }
//         else if (Globals.playerDirection == PlayerDirection.South)
//             {
//                 animator.SetTrigger("SwordSwingDown");
//             }
//         else if (Globals.playerDirection == PlayerDirection.East || Globals.playerDirection == PlayerDirection.West)
//         {
//             animator.SetTrigger("SwordSwingSide");
//         }




//         canSwing = false;
//         transform.FindChild("SwordPivot").gameObject.SetActive(true);
//         transform.FindChild("SwordPivot").gameObject.SendMessage("Swing", Globals.playerDirection);

//     }
// }

// //SHOOT
//else if (Input.GetKey(KeyCode.A) && Time.time > nextFire)
// {
//     nextFire = Time.time + fireRate;
//     if (gameController.DrainPlayerEnergy(energyToUseArrow))
//     {                


//         if (Globals.playerDirection == PlayerDirection.North)
//         {
//             Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
//             animator.SetTrigger("PlayerShootUp");
//         }
//         else if (Globals.playerDirection == PlayerDirection.South)
//         {
//             Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
//             animator.SetTrigger("PlayerShootDown");
//         }
//         else if (Globals.playerDirection == PlayerDirection.East || Globals.playerDirection == PlayerDirection.West)
//         {
//             Instantiate(shot, shotSpawn.position, Quaternion.Euler(0f, 0f, 90f));

//             animator.SetTrigger("PlayerShootSide");
//         }
//     }


// }

// //DASH
// if (Input.GetKeyDown(KeyCode.Space))
// {
//     if (gameController.DrainPlayerEnergy(energyToUseDash))
//     {
//         StartCoroutine(Dash());
//     }
// }

// //STOP SHIELD
// if (Input.GetKeyUp(KeyCode.S))
// {
//     transform.FindChild("Shield").gameObject.SetActive(false);
//     animator.SetBool("PlayerShieldDown", true);

// }