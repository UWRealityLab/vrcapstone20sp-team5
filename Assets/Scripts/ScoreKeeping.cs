using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeping : MonoBehaviour
{
    
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    private void OnTriggerEnter(Collider other) {
        Destroy(other.gameObject);
        score += 1;
        Debug.Log("Score: " + score);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
