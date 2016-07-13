using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour {
    public float startAngle;
    public float endAngle;
    public float tol;
    public float speed;
    public float startDelay;
    public float endDelay;

    private Vector3 start;
    private float totalDegrees;
    private float remainingDegrees;
    private float degreesTraveled;
    private float mySpeed;
    private BoxCollider2D sword;
    private Vector2 boxOffset;
    private Vector2 boxSize;

    private void Start()
    {
        sword = transform.FindChild("Sword").GetComponent<BoxCollider2D>();
        boxOffset = sword.offset;
        boxSize = sword.size;
    }

    public void Swing(PlayerDirection dir)
    {        
        StartCoroutine(SwingCoroutine(dir));
    }

    private IEnumerator SwingCoroutine(PlayerDirection dir)
    {
        if(dir == PlayerDirection.North)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, startAngle);
            mySpeed = speed;
        }
        else if (dir == PlayerDirection.South)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f + startAngle);
            mySpeed = speed;
        }
        else if (dir == PlayerDirection.West)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f + startAngle);
            mySpeed = speed;
        }
        else if (dir == PlayerDirection.East)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f + startAngle);
            mySpeed = -speed;
        }

        totalDegrees = Mathf.Abs(startAngle) + Mathf.Abs(endAngle);
        remainingDegrees = totalDegrees;
        yield return new WaitForSeconds(startDelay);
        while (remainingDegrees > tol)
        {
            degreesTraveled = mySpeed * Time.deltaTime;
            transform.RotateAround(transform.position, Vector3.forward, degreesTraveled);
            remainingDegrees -= Mathf.Abs(degreesTraveled);
            if (remainingDegrees <= tol)
            {
                sword.size = boxSize;
                sword.offset = boxOffset;

            }
            else if ((totalDegrees/remainingDegrees) < 2f)
            {
                sword.size = new Vector2(boxSize.x, boxSize.y * (totalDegrees / remainingDegrees));
                sword.offset = new Vector2(boxOffset.x, boxOffset.y * (totalDegrees / remainingDegrees));

            }         
            else
            {
                sword.size = new Vector2(boxSize.x, boxSize.y * (totalDegrees / (totalDegrees - remainingDegrees)));
                sword.offset = new Vector2(boxOffset.x, boxOffset.y * (totalDegrees / (totalDegrees - remainingDegrees)));

            }

            yield return null;
        }
        
        yield return new WaitForSeconds(endDelay);

        GameObject.Find("Player").SendMessage("PlayerCanSwing");

    }
}
