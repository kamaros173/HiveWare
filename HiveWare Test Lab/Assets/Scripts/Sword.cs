using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour {
    public float startAngle;
    public float endAngle;
    public float tol;
    public float speed;
    public float startDelay;
    public float endDelay;

    //private Vector3 target;
    private Vector3 start;
    private float remainingDegrees;
    private float degreesTraveled;


    public void Swing(PlayerDirection dir)
    {
        StartCoroutine(SwingCoroutine(dir));
    }

    private IEnumerator SwingCoroutine(PlayerDirection dir)
    {
        if(dir == PlayerDirection.North)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, startAngle);
            //target = new Vector3(0f, 0f, endAngle);
        }
        else if (dir == PlayerDirection.South)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f + startAngle);
            //target = new Vector3(0f, 0f, 180f + endAngle);
        }
        else if (dir == PlayerDirection.West)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f + startAngle);
            //target = new Vector3(0f, 0f, 90f + endAngle);
        }
        else if (dir == PlayerDirection.East)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f + startAngle);
            //target = new Vector3(0f, 0f, -90f + endAngle);
        }

        remainingDegrees = Mathf.Abs(startAngle) + Mathf.Abs(endAngle);
        yield return new WaitForSeconds(startDelay);
        while (remainingDegrees > tol)
        {
            degreesTraveled = speed * Time.deltaTime;
            transform.RotateAround(transform.position, Vector3.forward, degreesTraveled);
            remainingDegrees -= degreesTraveled;
            yield return null;
        }

        yield return new WaitForSeconds(endDelay);
        GameObject.Find("Player").SendMessage("PlayerCanSwing");

    }
}
