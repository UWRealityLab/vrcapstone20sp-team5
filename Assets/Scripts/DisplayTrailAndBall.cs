using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class DisplayTrailAndBall : MonoBehaviour
{

    public GameObject ball;
    public GameObject trail;
    public PathCreator path;
    public float speed;
    public float delay;
    private float travelDst;

    // Start is called before the first frame update
    void Start()
    {
        trail = Instantiate(trail, path.path.GetPointAtDistance(0), path.path.GetRotationAtDistance(0));
        travelDst = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // if the ball or the trail hits the end point, destory them
        if (trail != null && trail.transform.position == path.path.GetPointAtTime(1, EndOfPathInstruction.Stop))
        {
            Destroy(trail, 3);
        }
        if (ball == null || ball.transform.position == path.path.GetPointAtTime(1, EndOfPathInstruction.Stop)) 
        {
            // the ball can be null if it is caught and destoryed
            if (ball != null) Destroy(ball);
            Destroy(this.gameObject);
        }
        
        travelDst += Time.deltaTime * speed;
        if (trail != null) 
        {
            trail.transform.position = path.path.GetPointAtDistance(travelDst, EndOfPathInstruction.Stop);
        }

        float prevDelay = delay;
        delay -= Time.deltaTime;
        if (delay < 0)
        {
            // if this is the first time delay became less than 0, then instantiate the ball
            if (prevDelay >= 0) 
            {
                ball = Instantiate(ball, path.path.GetPointAtDistance(0), path.path.GetRotationAtDistance(0));
            }
            if (ball != null) 
            {
                ball.transform.position = path.path.GetPointAtDistance(-delay * speed, EndOfPathInstruction.Stop);
                ball.transform.rotation = path.path.GetRotationAtDistance(-delay * speed, EndOfPathInstruction.Stop);
            }
        }
    }
}
