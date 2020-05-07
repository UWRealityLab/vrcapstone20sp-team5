using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public GameObject ball;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        float distanceTravelled;
        public float delay;

        void Start() {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void Update()
        {
            if (pathCreator != null)
            {
                distanceTravelled += speed * Time.deltaTime;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                
                float prevDelay = delay;
                delay -= Time.deltaTime;
                if (delay < 0)
                {
                    // if this is the first time delay became less than 0, then instantiate the ball
                    if (prevDelay >= 0) 
                    {
                        ball = Instantiate(ball, pathCreator.path.GetPointAtDistance(0), pathCreator.path.GetRotationAtDistance(0));
                    }
                    if (ball != null) 
                    {
                        ball.transform.position = pathCreator.path.GetPointAtDistance(-delay * speed, EndOfPathInstruction.Stop);
                        ball.transform.rotation = pathCreator.path.GetRotationAtDistance(-delay * speed, EndOfPathInstruction.Stop);
                    }
                }
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}