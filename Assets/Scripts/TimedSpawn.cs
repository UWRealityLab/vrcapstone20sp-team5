using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpawn : MonoBehaviour
{
    public GameObject ball;
    public bool stopSpawning = false;
    public float spawnTime;
    public float spawnDelay = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }

    public void SpawnObject() 
    {
        GameObject spawner = (GameObject) Resources.Load("Spawner");
        Instantiate(spawner, transform.position, transform.rotation);

        if (stopSpawning) 
        {
            CancelInvoke("SpawnObject");
        }
    }
}
