using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            gameObject.SendMessageUpwards("StartChase");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            gameObject.SendMessageUpwards("StopChase");

    }
}
