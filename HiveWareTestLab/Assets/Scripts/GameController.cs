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
    public AudioClip playerFallClip;
    public AudioClip enemyFallClip;
    public AudioClip heartPopClip;
  
    private int playerHealth;
    private float playerEnergy;
    //private bool isEnergyDelayed = false;
    private HashSet<GameObject> currentEnemies = new HashSet<GameObject>();
    private HashSet<GameObject> deadEnemies = new HashSet<GameObject>();
    private GameObject player;
    private GameObject checkpoint;
    private GameObject[] pauseObjects;
    private SoundManager soundManager;
    private float delayedTime;
    private GameObject deathMenu;


    void Start()
    {
        Debug.Log("Start");
        playerHealth = playerHealthMax;
        playerEnergy = playerEnergyMax;
        player = GameObject.Find("Player");
        checkpoint = GameObject.Find("Checkpoint");
        delayedTime = 0f;

        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        deathMenu = GameObject.Find("DeathMenu");
        deathMenu.SetActive(false);
        HidePaused();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseControl();
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

    public bool IsThereEnemies()
    {
        return currentEnemies.Count != 0;
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
        StartCoroutine(HeartPop()); 
    }

    private IEnumerator HeartPop()
    {
        playerHealth = playerHealthMax;

        float timer = Time.time + 1f;
        foreach (Image h in hearts)
        {
            while(Time.time < timer)
            {
                yield return null;
            }

            if(h.sprite != heart)
            {
                timer = Time.time + 0.75f;
                h.sprite = heart;
                soundManager.RandomizeSfx(heartPopClip, 1f);
            }

            yield return null;
        }
    }

    public bool DrainPlayerEnergy(float amount)
    {
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

    private void PlayerInHole(Vector3 center)
    {
        StartCoroutine(PlayerFallSpin(center));
        //soundManager.PlaySingle(playerFallClip, 1f);       
    }

    private IEnumerator PlayerFallSpin(Vector3 center)
    {
        while(player.transform.localScale.y > 0.05)
        {
            player.transform.localScale = Vector3.Lerp(player.transform.localScale, Vector3.zero, Time.deltaTime);
            player.transform.Rotate(0f,0f,5f);
            player.transform.position = Vector3.Lerp(player.transform.position, center, 2f * Time.deltaTime);
            yield return null;
        }

        PlayerHasDied();
    }

    private void EnemyInHole(Transform[] points)
    {
        AddDeadEnemyToList(points[1].gameObject);
        StartCoroutine(EnemyFallSpin(points));
        soundManager.PlaySingle(enemyFallClip, 1f);

    }

    private IEnumerator EnemyFallSpin(Transform[] points)
    {
        while (points[1].transform.localScale.y > 0.05)
        {
            points[1].transform.localScale = Vector3.Lerp(points[1].transform.localScale, Vector3.zero, Time.deltaTime);
            points[1].transform.Rotate(0f, 0f, 5f);
            points[1].position = Vector3.Lerp(points[1].position, points[0].position, 2f * Time.deltaTime);
            yield return null;
        }
       
        points[1].gameObject.SetActive(false);
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
        deathMenu.SetActive(true);
    }

    public void respawnAtCheckpoint()
    {
        deathMenu.SetActive(false);
        foreach (Image health in hearts)
        {
            health.sprite = heart;
        }
        playerHealth = playerHealthMax;
        playerEnergy = playerEnergyMax;
        //isEnergyDelayed = false;
        player.SendMessage("Reset");
        player.transform.position = checkpoint.transform.position;
        player.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        player.transform.localScale = new Vector3(1f, 0.75f, 1f);
        Camera.main.transform.position = new Vector3(checkpoint.transform.position.x, checkpoint.transform.position.y, Camera.main.transform.position.z);
        ReviveDeadEnemies();
        GameObject.Find("Boss").SendMessage("Reset");
    }

    public void ShowPaused()
    {
        foreach (GameObject g in pauseObjects)
            g.SetActive(true);
    }

    public void HidePaused()
    {
        foreach (GameObject g in pauseObjects)
            g.SetActive(false);
    }

    public void PauseControl()
    {
        if (GameObject.Find("StartScreen") == null)
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                ShowPaused();
            }
            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                HidePaused();
            }
        }
    }

    public void GoToMainMenu()
    {
        player.SendMessage("Reset");
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameDungeonV10");
    }

    public void AddDeadEnemyToList(GameObject enemy)
    {
        deadEnemies.Add(enemy);
    }

    public void ReviveDeadEnemies()
    {
        foreach(GameObject enemy in deadEnemies)
        {
            enemy.SetActive(true);
            enemy.SendMessage("Resurrect");
        }

        ClearDeadEnemies();
    }

    public void ClearDeadEnemies()
    {
        deadEnemies.Clear();
    }

}
