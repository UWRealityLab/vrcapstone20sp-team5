using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeping : MonoBehaviour
{
    
    public int score;
    public int up;
    public int medium;
    public int down;
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Ball") {
            score += 1; // total score increment

            // check ball collision position to categorize exercise
            if (other.gameObject.transform.position.y >= 0.0) {
                up +=  1;
            } else if (other.gameObject.transform.position.y <= -0.70) {
                down += 1;
            } else {
                medium += 1;
            }

            Destroy(other.gameObject); // destroy immediately to avoid second collision
        }
    }

}
