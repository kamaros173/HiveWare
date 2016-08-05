using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float smoothing;
    public float errorTol;
    public float cameraViewHeight;
    public float cameraViewWidth;
    public float playerSmoothing;

    private Vector3 destination;
    private GameObject player;
    private GameObject gc;

	void Start () {
        player = GameObject.Find("Player");
        gc = GameObject.Find("GameController");
        // TODO: ACCOUNT FOR DIFFERENT SCREEN RESOLUTIONS
    }
	
    private void MoveCamera(PlayerDirection dir)
    {
        Globals.notFrozen = false;
        gc.SendMessage("ClearEnemies");
        Vector3 cameraTarget = transform.position;
        Vector3 playerTarget = player.transform.position;
        float xfinal = player.transform.position.x - cameraTarget.x;
        float yfinal = player.transform.position.y - cameraTarget.y;

        if (Mathf.Abs(xfinal) > Mathf.Abs(yfinal * (cameraViewWidth / cameraViewHeight)))
        {
            if (xfinal > 0)
            {
                cameraTarget.x += cameraViewWidth;
                playerTarget.x += 0.75f;
            }
            else
            {
                cameraTarget.x -= cameraViewWidth;
                playerTarget.x -= 0.75f;
            }
        }
        else
        {
            if (yfinal > 0)
            {
                cameraTarget.y += cameraViewHeight;
                playerTarget.y += 0.75f;
            }
            else
            {
                cameraTarget.y -= cameraViewHeight;
                playerTarget.y -= 0.75f;
            }
        }

        StartCoroutine(MoveCoroutine(cameraTarget, playerTarget));
    }

    private IEnumerator MoveCoroutine (Vector3 cameraTarget, Vector3 playerTarget)
    {
        
        while (Vector3.Distance(transform.position, cameraTarget) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, cameraTarget, smoothing * Time.deltaTime);
            player.transform.position = Vector3.Lerp(player.transform.position, playerTarget, playerSmoothing * Time.deltaTime);           
            yield return null;
        }

        transform.position = new Vector3(cameraTarget.x, cameraTarget.y, cameraTarget.z);
        Globals.notFrozen = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Projectile")
        {
            GameObject.Destroy(other.gameObject);
        }
    }

    private IEnumerator Shake(float length)
    {
        Vector3 posCenter = transform.position;
        Vector3 posRight = posCenter + new Vector3(0.1f, 0f, 0f);
        Vector3 posLeft = posCenter + new Vector3(-0.1f, 0f, 0f);
        float timer = Time.time + length;
        bool goingRight = true;


        while(timer > Time.time)
        {
            if (goingRight)
            {
                transform.Translate(Vector3.right * 0.1f);
                if (transform.position.x >= posRight.x)
                    goingRight = false;
            }
            else
            {
                transform.Translate(Vector3.left * 0.1f);
                if (transform.position.x <= posLeft.x)
                    goingRight = true;
            }

            yield return null;
        }

        transform.position = posCenter;
    }
}
