using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class DisplayTrailAndBall: MonoBehaviour
{

    public GameObject ball;
    public GameObject trail;
    public Vector3 start;
    public Vector3 end;
    public Vector3 middle;
    public float speed;
    public float delay;
    private float travelDst;
    private VertexPath path;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] points = {start, middle, end};
        path = new VertexPath(new BezierPath(points), transform);
        trail = Instantiate(trail, path.GetPointAtDistance(0), path.GetRotationAtDistance(0));
        travelDst = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // if the ball or the trail hits the end point, destory them
        if (trail != null && trail.transform.position == path.GetPointAtTime(1, EndOfPathInstruction.Stop))
        {
            Destroy(trail, 3);
        }
        if (ball == null || ball.transform.position == path.GetPointAtTime(1, EndOfPathInstruction.Stop)) 
        {
            // the ball can be null if it is caught and destoryed
            if (ball != null) Destroy(ball);
            Destroy(this.gameObject);
        }
        
        travelDst += Time.deltaTime * speed;
        if (trail != null) 
        {
            trail.transform.position = path.GetPointAtDistance(travelDst, EndOfPathInstruction.Stop);
        }
        float prevDelay = delay;
        delay -= Time.deltaTime;
        if (delay < 0)
        {
            // if this is the first time delay became less than 0, then instantiate the ball
            if (prevDelay >= 0) 
            {
                ball = Instantiate(ball, path.GetPointAtDistance(0), path.GetRotationAtDistance(0));
            }
            if (ball != null) 
            {
                ball.transform.position = path.GetPointAtDistance(-delay * speed, EndOfPathInstruction.Stop);
                ball.transform.rotation = path.GetRotationAtDistance(-delay * speed, EndOfPathInstruction.Stop);
            }
        }
    }
}
