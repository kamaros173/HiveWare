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
    private float remainingDegrees;
    private float degreesTraveled;
    private float mySpeed;

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

        remainingDegrees = Mathf.Abs(startAngle) + Mathf.Abs(endAngle);
        yield return new WaitForSeconds(startDelay);
        while (remainingDegrees > tol)
        {
            degreesTraveled = mySpeed * Time.deltaTime;
            transform.RotateAround(transform.position, Vector3.forward, degreesTraveled);
            remainingDegrees -= Mathf.Abs(degreesTraveled);
            yield return null;
        }
        
        yield return new WaitForSeconds(endDelay);

        GameObject.Find("Player").SendMessage("PlayerCanSwing");

    }
}
