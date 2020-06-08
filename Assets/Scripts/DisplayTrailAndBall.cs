using UnityEngine;
using PathCreation;
using static System.Math;

public class DisplayTrailAndBall: MonoBehaviour
{

    public GameObject holePrefabStart;
    public GameObject holePrefabEnd;
    public Quaternion startRotation;
    public Quaternion endRotation;
    public Vector3 start;
    public Vector3 end;
    public Vector3 middle;
    public float speed;
    public float delay;
    public GameObject[] balls;
    public GameObject[] trails;
    private GameObject ball;
    private GameObject trail;
    private int random;
    private float travelDst;
    private VertexPath path;
    private GameObject startHole;
    private GameObject endHole;
    public ScoreKeeping scorekeeper;
    public bool survival;

    public void setParams(Vector3 start, Vector3 end, Quaternion startRotation, Quaternion endRotation, Vector3 middle) {
        this.start = start;
        this.end = end;
        this.startRotation = startRotation;
        this.endRotation = endRotation;
        this.middle = middle; 
    }


    // Start is called before the first frame update
    void Start()
    {
        // randomly pick a color and get the trail/ball combination for the color
        if (survival) {
            random = Random.Range(0, Min(balls.Length, trails.Length));
        } else {
            random = Random.Range(1, Min(balls.Length, trails.Length));
        }
        trail = trails[random];
        ball = balls[random];
        
        Vector3[] points = {start, middle, end};
        path = new VertexPath(new BezierPath(points), transform);
        trail = Instantiate(trail, path.GetPointAtDistance(0), path.GetRotationAtDistance(0));
        startHole = Instantiate(holePrefabStart, start, startRotation);
        endHole = Instantiate(holePrefabEnd, end, endRotation);
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
            if (ball != null && survival) {
                if (ball.tag != "Bomb" && scorekeeper.caught) {
                    scorekeeper.caught = false;
                } else {
                    scorekeeper.lives--;
                }
            }
            // the ball can be null if it is caught and destoryed
            if (ball != null) Destroy(ball);
            Destroy(startHole);
            Destroy(endHole);
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
                // mark end of path since ball.transform.position is not accurate
                ball.GetComponent<Explosion>().end = this.end; 
                Instantiate(GameObject.Find("AudioManager").GetComponent<AudioManager>().spawn,
                    start,Quaternion.identity);
            }
            if (ball != null) 
            {
                ball.transform.position = path.GetPointAtDistance(-delay * speed, EndOfPathInstruction.Stop);
                ball.transform.rotation = path.GetRotationAtDistance(-delay * speed, EndOfPathInstruction.Stop);
            }
        }
    }

    // called when game is disabled, distory all existing objects
    public void Finish() {
        if (trail != null) Destroy(trail);
        if (ball != null) Destroy(ball);
        Destroy(startHole);
        Destroy(endHole);
        Destroy(this.gameObject);
    }
}
