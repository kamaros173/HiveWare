using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    public Image[] hearts;
    public Sprite skull;
    public Sprite heart;
    public int playerHealthMax;
    public float playerEnergyMax;
    public float energyPerSecond;
    public float energyDelay;
    public Slider energyBar;
    public GameObject playerPrefab;
  

    private int playerHealth;
    private float playerEnergy;
    private bool isEnergyDelayed = false;
    private HashSet<GameObject> currentEnemies = new HashSet<GameObject>();
    private GameObject player;
    private GameObject checkpoint;
    private float delayedTime;


    void Start()
    {
        playerHealth = playerHealthMax;
        playerEnergy = playerEnergyMax;
        player = GameObject.Find("Player");
        checkpoint = GameObject.Find("Checkpoint");
        delayedTime = 0f;
    }

    void Update()
    {
        if (delayedTime <= 0f)
        {
            delayedTime = 0f;
            if (playerEnergy + (energyPerSecond * Time.deltaTime) > playerEnergyMax)
            {
                playerEnergy = playerEnergyMax;
                energyBar.value = playerEnergy;
            }
            else
            {
                playerEnergy += energyPerSecond * Time.deltaTime;
                energyBar.value = playerEnergy;
            }
        }
        else if(delayedTime > energyDelay)
        {
            delayedTime = energyDelay;
        }
        else
        {
            delayedTime -= Time.deltaTime;
        }
    }

    private void AddEnemy(GameObject enemy)
    {
        currentEnemies.Add(enemy);
    }

    private void ClearEnemies()
    {
        foreach(GameObject enemy in currentEnemies)
            enemy.SendMessage("Reset");

        currentEnemies.Clear();
    }

    private void RemoveEnemy(GameObject enemy)
    {
        currentEnemies.Remove(enemy);
    }

    private void HurtPlayer(Vector3 direction)
    {
        if (!Globals.isPlayerDead)
        {
            playerHealth--;

            if (playerHealth >= 0)
            {
                hearts[playerHealth].sprite = skull;
            }
            if (playerHealth <= 0)
            {
                Globals.notFrozen = false;
                Globals.isPlayerDead = true;
                GameObject.Find("Player").SendMessage("PlayerDeath");
                PlayerHasDied();
            }
            else
            {
                GameObject.Find("Player").SendMessage("HitPlayer", direction);
            }
        }
    }

    private void HealPlayer()
    {
        playerHealth++;
        if(playerHealth > playerHealthMax)
        {
            playerHealth = playerHealthMax;
        }
    }

    public bool DrainPlayerEnergy(float amount)
    {
        //StartCoroutine(DelayEnergyRegen());
        delayedTime += energyDelay;
        if((playerEnergy - amount) < 0)
        {
            Debug.Log("Not Enough Energy: " + playerEnergy + " left");
            return false;
        }

        playerEnergy -= amount;
        energyBar.value = playerEnergy;
        return true;
    }

    //private IEnumerator DelayEnergyRegen()
    //{
    //    isEnergyDelayed = true;
    //    float remaining = Time.time + energyDelay;

    //    while(remaining > Time.time)
    //    {
    //        yield return null;
    //    }

    //    isEnergyDelayed = false;
    //}

    private void PlayerInHole(Vector3 center)
    {
        StartCoroutine(PlayerFallSpin(center));       
    }

    //private IEnumerator PlayerFalling(Vector3 center)
    //{
    //    while(Vector3.Distance(center, GameObject.Find("Player").transform.position) > 0.05f){
    //        GameObject.Find("Player").transform.position = Vector3.Lerp(GameObject.Find("Player").transform.position, center, 2f*Time.deltaTime);
    //        yield return null;
    //    }
    //}

    private IEnumerator PlayerFallSpin(Vector3 center)
    {
        while(player.transform.localScale.y > 0.05)
        {
            player.transform.localScale = Vector3.Lerp(player.transform.localScale, Vector3.zero, Time.deltaTime);
            player.transform.Rotate(0f,0f,5f);
            player.transform.position = Vector3.Lerp(player.transform.position, center, 2f * Time.deltaTime);
            yield return null;
        }

        respawnAtCheckpoint();
    }

    private void EnemyInHole(Transform[] points)
    {
        //StartCoroutine(EnemyFalling(points));
        StartCoroutine(EnemyFallSpin(points));
    }

    //private IEnumerator EnemyFalling(Transform[] points)
    //{
    //    Debug.Log("EnemyFalling");
    //    while (Vector3.Distance(points[0].position, points[1].position) > 0.05f)
    //    {
    //        points[1].position = Vector3.Lerp(points[1].position, points[0].position, 2f * Time.deltaTime);
    //        yield return null;
    //    }
    //    Debug.Log("EnemyFalling");
    //}

    private IEnumerator EnemyFallSpin(Transform[] points)
    {
        while (points[1].transform.localScale.y > 0.05)
        {
            points[1].transform.localScale = Vector3.Lerp(points[1].transform.localScale, Vector3.zero, Time.deltaTime);
            points[1].transform.Rotate(0f, 0f, 5f);
            points[1].position = Vector3.Lerp(points[1].position, points[0].position, 2f * Time.deltaTime);
            yield return null;
        }

        Destroy(points[1].gameObject);
    }

    private void UnfreezePlayer()
    {
        if (!Globals.isPlayerDead)
        {
            Globals.notFrozen = true;
        }
    }

    private void PlayerHasDied()
    {
        //Load UI SCREEN WITH OPTIONS
        respawnAtCheckpoint();
    }

    private void respawnAtCheckpoint()
    {
        foreach (Image health in hearts)
        {
            health.sprite = heart;
        }
        playerHealth = playerHealthMax;
        playerEnergy = playerEnergyMax;
        isEnergyDelayed = false;
        player.SendMessage("Reset");
        player.transform.position = checkpoint.transform.position;
        player.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        player.transform.localScale = new Vector3(1f, 0.75f, 1f);
        Camera.main.transform.position = new Vector3(checkpoint.transform.position.x, checkpoint.transform.position.y, Camera.main.transform.position.z);
    }

}
