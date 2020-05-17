using UnityEngine;
using MagicLeapTools;

public class ScoreKeeping : MonoBehaviour
{
    
    [HideInInspector]
    public int score;

    [HideInInspector]
    public int up;

    [HideInInspector]
    public int medium;
    
    [HideInInspector]
    public int down;

    [HideInInspector]
    public float timer;

    [HideInInspector]
    public enum ChangeType { Up, Middle, Down, Reset };

    [HideInInspector]
    public delegate void ScoreChangeNotifier(int change, ChangeType type);

    public event ScoreChangeNotifier ScoreChange; 

    public float upLimit = 1.7f;
    public float downLimit = 0.6f;

    private void Update() {
        timer += Time.deltaTime;
    }
    
    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Ball") {
            score++;
            float floorHeight = Playspace.Instance.FloorCenter.y;
            float collisionHeight = collision.contacts[0].point.y;
            
            // check ball collision position to categorize exercise
            float height = collisionHeight - floorHeight;
            ChangeType type;
            if (height > upLimit) {
                up += 1;
                type = ChangeType.Up;
            } else if (height < downLimit) {
                down += 1;
                type = ChangeType.Down;
            } else {
                medium += 1;
                type = ChangeType.Middle;
            }

            // Notify all event handler of ScoreChange
            ScoreChange?.Invoke(1, type);
            // destroy is handled in explosion script
        }
    }

    public void ResetScore() {
        score = 0;
        up = 0; 
        medium = 0;
        down = 0;
        timer = 0;
        ScoreChange?.Invoke(-1, ChangeType.Reset);
    }
}
