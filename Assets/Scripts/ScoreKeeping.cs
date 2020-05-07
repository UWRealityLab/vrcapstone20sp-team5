using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicLeapTools;

public class ScoreKeeping : MonoBehaviour
{
    
    public int score;
    public int up;
    public int medium;
    public int down;
    public Text status;
    private float _floorHeight;
    private float _controllerHeight;
    
    private void OnTriggerEnter(Collider other) {
        _floorHeight = Playspace.Instance.FloorCenter.y;

        if (other.gameObject.tag == "Ball") {
            score += 1; // total score increment
            // check ball collision position to categorize exercise
            _controllerHeight =  this.transform.position.y;

            if (_controllerHeight - _floorHeight >= 1.70f) {
                up +=  1;
                status.text = "up-stretch +1!";
            } else if (_controllerHeight - _floorHeight <= 0.60f) {
                down += 1;
                status.text = "down-squat +1!";
            } else {
                medium += 1;
                status.text = "Good Job!";
            }

            Destroy(other.gameObject); // destroy immediately to avoid second collision
        }
    }

    private void OnDestroy() {
        status.text = "";
    }

}
