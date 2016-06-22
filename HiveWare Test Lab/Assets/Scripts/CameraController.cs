using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float smoothing;
    public float errorTol;
    public float cameraViewHeight;
    public float cameraViewWidth;
    public float widthToBeSeen;

    private Vector3 destination;

	// Use this for initialization
	void Start () {
        Camera.main.orthographicSize = widthToBeSeen * Screen.height / Screen.width * 0.5f;
    }
	
	// Update is called once per frame
	void Update () {

    }

    private void MoveCamera(PlayerDirection dir)
    {
        Globals.notFrozen = false;
        GameObject.Find("GameController").SendMessage("ClearEnemies");
        Vector3 target = transform.position;
        if(dir == PlayerDirection.North)
        {
            target.y += cameraViewHeight;
        }
        else if (dir == PlayerDirection.South)
        {
            target.y -= cameraViewHeight;
        }
        else if (dir == PlayerDirection.East)
        {
            target.x += cameraViewWidth;
        }
        else if (dir == PlayerDirection.West)
        {
            target.x -= cameraViewWidth;
        }

        StartCoroutine(MoveCoroutine(target));
    }

    private IEnumerator MoveCoroutine (Vector3 target)
    {
        
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, target, smoothing * Time.deltaTime);
            yield return null;
        }

        transform.position = new Vector3(target.x, target.y, target.z);
        Globals.notFrozen = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Projectile")
        {
            GameObject.Destroy(other.gameObject);
        }
    }
}
