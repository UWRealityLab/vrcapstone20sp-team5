using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Follower : MonoBehaviour
{
    public PathCreator path;
    public GameObject trail;

    public float speed;
    public float delay;
    float travelDst;

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

        if (this.gameObject == null || this.gameObject.transform.position == path.path.GetPointAtTime(1, EndOfPathInstruction.Stop)) 
        {
            // the ball can be null if it is caught and destroyed
            if (this.gameObject != null) 
            {
                Destroy(this.gameObject);
            } 
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
                Instantiate(this.gameObject, path.path.GetPointAtDistance(0), path.path.GetRotationAtDistance(0));
            }

            if (this.gameObject != null) 
            {
                this.gameObject.transform.position = path.path.GetPointAtDistance(-delay * speed, EndOfPathInstruction.Stop);
                this.gameObject.transform.rotation = path.path.GetRotationAtDistance(-delay * speed, EndOfPathInstruction.Stop);
            }
        }
    }
}
