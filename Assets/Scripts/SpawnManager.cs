using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using static System.Math;

namespace PathCreation {
    public class SpawnManager : MonoBehaviour {

    #region Public Variables  
    public PlaneController PlaneScript;
    public GameObject TrailAndBall;
    public float SpawnFrequency = 10f;
    public Text ScanText;
    #endregion
    
    #region Private Variables
    private int numPlane;
    private int spawnCount;
    private float timer = 0.0f;
    private bool init = false;
    #endregion
    
    #region Unity Methods
    private void Start() {
        
    }
    private void Update() {
        timer += Time.deltaTime;

        if (PlaneScript.Completed) {
            if (!init) { // initialization if plane extraction is done
                init = true;
                numPlane = PlaneScript.planeCache.Count;
                spawnCount = 0;
            }

            if (timer > SpawnFrequency) { // spawn a ball every 10 seconds
                timer = 0.0f;
                spawnCount++;
                RandomSpawn2();
                ScanText.text = "Spawned " + spawnCount.ToString();
            }
            
        }
    }
    private void OnDestroy () {
        
    }
    #endregion
    
    #region Private Methods

    // first select a random plane
    // then randomly select one point on the plane to spawn the ball
    private void RandomSpawn2() {

        GameObject planeStart = PlaneScript.planeCache[Random.Range(0, numPlane)];
        GameObject planeEnd = planeStart;
        while (planeEnd == planeStart) planeEnd = PlaneScript.planeCache[Random.Range(0, numPlane)];
        DisplayTrailAndBall spawner = TrailAndBall.GetComponent<DisplayTrailAndBall>();

        float xStart = Random.Range(planeStart.transform.position.x - planeStart.transform.localScale.x/2,
            planeStart.transform.position.x + planeStart.transform.localScale.x/2);
        float yStart = Random.Range(planeStart.transform.position.y - planeStart.transform.localScale.y/2,
            planeStart.transform.position.y + planeStart.transform.localScale.y/2);
        float zStart = Random.Range(planeStart.transform.position.z - planeStart.transform.localScale.z/2,
            planeStart.transform.position.z + planeStart.transform.localScale.z/2);
        Collider collStart = planeStart.GetComponent<Collider>();
        Vector3 startLoc = collStart.ClosestPointOnBounds(new Vector3(xStart, yStart, zStart));

        float xEnd = Random.Range(planeEnd.transform.position.x - planeEnd.transform.localScale.x/2,
            planeEnd.transform.position.x + planeEnd.transform.localScale.x/2);
        float yEnd = Random.Range(planeEnd.transform.position.y - planeEnd.transform.localScale.y/2,
            planeEnd.transform.position.y + planeEnd.transform.localScale.y/2);
        float zEnd = Random.Range(planeEnd.transform.position.z - planeEnd.transform.localScale.z/2,
            planeEnd.transform.position.z + planeEnd.transform.localScale.z/2);
        Collider collEnd = planeEnd.GetComponent<Collider>();
        Vector3 endLoc = collEnd.ClosestPointOnBounds(new Vector3(xEnd, yEnd, zEnd));


        // float xmin = 10f, xmax = -10f, ymin = 10f, ymax = -10f, zmin = 10f, zmax = -10f;
        // int StartPlaneIndex = 0;
        // GameObject StartPlane = PlaneScript.planeCache[StartPlaneIndex];
        
        // // choose a plane within the range
        // while (xmin > 4f || xmax < -4f || ymin > 0.3f ||  zmin > 4f || zmax < -4f) {
        //     StartPlaneIndex = Random.Range(0, numPlane);
        //     StartPlane = PlaneScript.planeCache[StartPlaneIndex];
        //     xmin = StartPlane.transform.position.x - StartPlane.transform.localScale.x/2;
        //     xmax = StartPlane.transform.position.x + StartPlane.transform.localScale.x/2;
        //     ymin = StartPlane.transform.position.y - StartPlane.transform.localScale.y/2;
        //     ymax = StartPlane.transform.position.y + StartPlane.transform.localScale.y/2;
        //     zmin = StartPlane.transform.position.z - StartPlane.transform.localScale.z/2;
        //     zmax = StartPlane.transform.position.z + StartPlane.transform.localScale.z/2;
        // }

        // // make sure spawn point is not too high/low/far
        // float StartX = Random.Range(Max(xmin, -4f), Min(4f, xmax));
        // float StartY = Random.Range(ymin, Min(.5f, ymax));
        // float StartZ = Random.Range(Max(zmin, -4f), Min(4f, zmax));


        spawner.start = startLoc;
        spawner.end = endLoc;
        spawner.middle = (startLoc + endLoc) / 2;

        // set the startPlane in explosion script 
        // so ball would not explode at the starting point
        spawner.ball.GetComponent<Explosion>().startPlane = planeStart;
        spawner.ball.GetComponent<Explosion>().endPlane= planeEnd;
        
        Instantiate(TrailAndBall, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        
        
    }

    #endregion
    }
}
