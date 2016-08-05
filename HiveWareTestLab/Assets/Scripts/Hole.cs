using UnityEngine;
using System.Collections;

public class Hole : MonoBehaviour {

    private GameObject gc;

    private void Start()
    {
        gc = GameObject.Find("GameController");
    }

	private void PlayerHasBeenHit()
    {
        gc.SendMessage("PlayerInHole", transform.position);
    }

    private void EnemyHasBeenHit(Transform enemy)
    {

        Transform[] temp = new Transform[2];
        temp[0] = transform;
        temp[1] = enemy;
        gc.SendMessage("EnemyInHole", temp);

    }
}
