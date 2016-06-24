using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float smoothing;
    public float errorTol;
    public float cameraViewHeight;
    public float cameraViewWidth;

    private Vector3 destination;

	void Start () {
        // TODO: ACCOUNT FOR DIFFERENT SCREEN RESOLUTIONS
    }
	
    private void MoveCamera(PlayerDirection dir)
    {
        Globals.notFrozen = false;
        GameObject.Find("GameController").SendMessage("ClearEnemies");
        Vector3 target = transform.position;
        Vector3 player = GameObject.Find("Player").transform.position;
        float xfinal = player.x - target.x;
        float yfinal = player.y - target.y;

        if(Mathf.Abs(xfinal) > Mathf.Abs(yfinal * (cameraViewWidth / cameraViewHeight)))
        {
            if(xfinal > 0)
                target.x += cameraViewWidth;
            else
                target.x -= cameraViewWidth;
        }
        else
        {
            if(yfinal > 0)
                target.y += cameraViewHeight;
            else
                target.y -= cameraViewHeight;
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
