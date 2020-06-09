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
    public int spawnCount;

    [HideInInspector]
    public float timer;

    [HideInInspector]
    public bool caught;

    [HideInInspector]
    public enum ChangeType { Up, Middle, Down, Reset };

    [HideInInspector]
    public delegate void ScoreChangeNotifier(int change, ChangeType type);

    [HideInInspector]
    public int lives;

    [HideInInspector]
    public int livesSetting = 3;

    public event ScoreChangeNotifier ScoreChange; 

    public float upLimit = 1.7f;
    public float downLimit = 0.6f;

    private bool played = false;

    private void OnEnable() {
        lives = 3;
    }
    private void Update() {
        timer += Time.deltaTime;
        if (lives <= 0 && !played) {
            Instantiate(GameObject.Find("AudioManager").GetComponent<AudioManager>().gameEnd,
                Playspace.Instance.Center,Quaternion.identity);
            played = true;
        }
    }
    
    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Ball") {
            score++;
            caught = true;
            //spawnCount++;
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

        if (collision.collider.tag == "Bomb") {
            lives = 0;
        }
    }

    public void ResetScore() {
        score = 0;
        up = 0; 
        medium = 0;
        down = 0;
        timer = 0;
        spawnCount = 0;
        caught = false;
        lives = livesSetting;
        played = false;
        ScoreChange?.Invoke(-1, ChangeType.Reset);
    }
}
