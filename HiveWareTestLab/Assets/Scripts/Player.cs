﻿using UnityEngine;
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


    private bool canSwing = true;
    private PlayerDirection currentdirection = PlayerDirection.North;
    private float nextFire;
    private Animator animator;

    void Start()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update () {

        if (Globals.notFrozen)
        {
            //MOVE
            if (Input.GetKey(KeyCode.UpArrow))
            {
                currentdirection = PlayerDirection.North;
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
                animator.SetTrigger("PlayerWalkUp");
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                animator.SetTrigger("PlayerIdleUp");
            }
          
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                currentdirection = PlayerDirection.South;
                //transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
                animator.SetTrigger("PlayerWalkDown");

            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                animator.SetTrigger("PlayerIdleUp");
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                currentdirection = PlayerDirection.East;
                transform.rotation = Quaternion.Euler(0f, 0f, 270f);
                transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);

            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                currentdirection = PlayerDirection.West;
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
            }

            //SHIELD (CURRENTLY ALLOWING SHIELD, SWORD, AND DASH AT THE SAME TIME)
            if (Input.GetKey(KeyCode.S))
            {
                transform.FindChild("Shield").gameObject.SetActive(true);
                animator.SetBool("PlayerShieldDown", false);
                animator.SetTrigger("PlayerShieldUp");

            }

            //SWORD
            if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D))
            {
                if (canSwing)
                {
                    animator.SetTrigger("SwordSwingUP");

                 
                    canSwing = false;
                    transform.FindChild("SwordPivot").gameObject.SetActive(true);
                    transform.FindChild("SwordPivot").gameObject.SendMessage("Swing", currentdirection);
                    
                }
            }

            //SHOOT
            if (Input.GetKey(KeyCode.A) && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                animator.SetTrigger("PlayerShootUp");

            }

            //DASH
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(Dash());
            }

            //STOP SHIELD
            if (Input.GetKeyUp(KeyCode.S))
            {
                transform.FindChild("Shield").gameObject.SetActive(false);
                animator.SetBool("PlayerShieldDown", true);
                
            }
        } // Not Frozen
        else // If Frozen
        {
            transform.FindChild("Shield").gameObject.SetActive(false);
        }

    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "MainCamera")
        {
            GameObject.Find("Main Camera").SendMessage("MoveCamera", currentdirection);     
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
            if (currentdirection == PlayerDirection.North)
            {
                target = transform.position + Vector3.up * dashDistance;
                dir = Vector2.up;
            }
            else if (currentdirection == PlayerDirection.South)
            {
                target = transform.position + Vector3.down * dashDistance;
                dir = Vector2.down;
            }
            else if (currentdirection == PlayerDirection.East)
            {
                target = transform.position + Vector3.right * dashDistance;
                dir = Vector2.right;
            }
            else if (currentdirection == PlayerDirection.West)
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
        //CAN ADD DAMAGE HERE
        Globals.playerIsHittable = false;
        Globals.notFrozen = false;
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
        RaycastHit2D hit;
        float oldDis = 0;

        do
        {
            oldDis = Vector3.Distance(transform.position, target);

            hit = Physics2D.Raycast(transform.position, target, oldDis, wallLayer);
            if (hit.collider != null)
                target = hit.point;

            transform.position = Vector3.Lerp(transform.position, target, pushBackSmooth * Time.deltaTime);
            yield return null;

        } while (oldDis > pushBackTol);

        Globals.notFrozen = true;
    }

    private void PlayerCanSwing()
    {
        transform.FindChild("SwordPivot").gameObject.SetActive(false);
        canSwing = true;
    }
}
