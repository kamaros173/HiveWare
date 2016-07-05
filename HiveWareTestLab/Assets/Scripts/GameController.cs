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
    

    private int playerHealth;
    private float playerEnergy;
    private bool isEnergyDelayed = false;
    private HashSet<GameObject> currentEnemies = new HashSet<GameObject>();
    private GameObject player;

    void Start()
    {
        playerHealth = playerHealthMax;
        playerEnergy = playerEnergyMax;
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if(!isEnergyDelayed){
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

        playerHealth--;

        if (playerHealth>=0)
        {
            hearts[playerHealth].sprite = skull;
        }

        Debug.Log("Controller Says: Player Hit - " + playerHealth + " hits remaining");
        if(playerHealth <= 0)
        {
            Debug.Log("Controller Says: Player has Died");
        }
        else
        {           
           GameObject.Find("Player").SendMessage("HitPlayer", direction);
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
        StartCoroutine(DelayEnergyRegen());
        if((playerEnergy - amount) < 0)
        {
            Debug.Log("Not Enough Energy: " + playerEnergy + " left");
            return false;
        }

        playerEnergy -= amount;
        energyBar.value = playerEnergy;
        return true;
    }

    private IEnumerator DelayEnergyRegen()
    {
        isEnergyDelayed = true;
        float remaining = Time.time + energyDelay;

        while(remaining > Time.time)
        {
            yield return null;
        }

        isEnergyDelayed = false;
    }

    private void PlayerInHole(Vector3 center)
    {
        Debug.Log("Player fell in hole");
        StartCoroutine(PlayerFalling(center));
        StartCoroutine(PlayerFallSpin());        
    }

    private IEnumerator PlayerFalling(Vector3 center)
    {
        while(Vector3.Distance(center, GameObject.Find("Player").transform.position) > 0.05f){
            GameObject.Find("Player").transform.position = Vector3.Lerp(GameObject.Find("Player").transform.position, center, 2f*Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator PlayerFallSpin()
    {
        while(player.transform.localScale.y > 0.05)
        {
            player.transform.localScale = Vector3.Lerp(player.transform.localScale, Vector3.zero, Time.deltaTime);
            player.transform.Rotate(0f,0f,5f);
            yield return null;

        }
    }
	
}
