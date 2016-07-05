using UnityEngine;
using System.Collections;

public class WallSpike : MonoBehaviour {

    [HideInInspector] public enum SpikeDirection
    {
        North,
        South,
        East,
        West
    }
    public SpikeDirection facing;
    
    //private Vector3 player;
    private Vector3 direction;

    private void Start()
    {
        //player = GameObject.Find("Player").transform.position;
        if (facing == SpikeDirection.North)
            direction = Vector3.up;
        else if (facing == SpikeDirection.South)
            direction = Vector3.down;
        else if (facing == SpikeDirection.East)
            direction = Vector3.right;
        else if (facing == SpikeDirection.West)
            direction = Vector3.left;

    }

    private void PlayerHasBeenHit()
    {
        GameObject.Find("GameController").SendMessage("HurtPlayer", direction);
        //GameObject.Find("GameController").SendMessage("HurtPlayer", Vector3.Normalize(player - transform.position));
    }
}
