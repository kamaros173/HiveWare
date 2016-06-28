using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    public int playerHealthMax;
    public float playerEnergyMax;
    public float energyPerSecond;
    public float energyDelay;

    private int playerHealth;
    private float playerEnergy;
    private bool isEnergyDelayed = false;
    private HashSet<GameObject> currentEnemies = new HashSet<GameObject>();

    void Start()
    {
        playerHealth = playerHealthMax;
        playerEnergy = playerEnergyMax;
    }

    void Update()
    {
        if(!isEnergyDelayed){
            if (playerEnergy + (energyPerSecond * Time.deltaTime) > playerEnergyMax)
            {
                playerEnergy = playerEnergyMax;
            }
            else
            {
                playerEnergy += energyPerSecond * Time.deltaTime;
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

    private void DrainPlayerEnergy(float amount)
    {
        StartCoroutine(DelayEnergyRegen());
        playerEnergy -= amount;
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
	
}
