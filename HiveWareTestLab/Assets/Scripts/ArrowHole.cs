using UnityEngine;
using System.Collections;

public class ArrowHole : MonoBehaviour {
    
    public float timeBetweenShots;
    public GameObject shot;
    public bool doNotAddToGC;
    public AudioClip shoot;
    public float delayStart;

    private float nextShot;
    private bool shooting;
    private Animator animator;
    private SoundManager soundManager;


    private void Start()
    {
        animator = GetComponent<Animator>();
        nextShot = Time.time + delayStart;
        if (doNotAddToGC)
            shooting = true;
        else
            shooting = false;
        
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    private void Update () {

        if (nextShot < Time.time && shooting)
        {
            nextShot = Time.time + timeBetweenShots;
            Instantiate(shot, transform.position, transform.rotation);
            soundManager.RandomizeSfx(shoot, 1f, 0.1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.tag == "MainCamera" && !doNotAddToGC)
        {
            shooting = true;
            GameObject.Find("GameController").SendMessage("AddEnemy", transform.gameObject);
            Load();
        }
    }

    //CALLED WHEN CAMERA MOVES AWAY FROM SCENE
    private void Reset()
    {
        shooting = false;
    }

    //CALLED WHEN CAMERA ENTERS ROOM
    private void Load()
    {
        nextShot = Time.time + delayStart;
        shooting = true;
    }
}
