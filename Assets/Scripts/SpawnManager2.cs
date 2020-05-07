using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using MagicLeapTools;


namespace PathCreation {
    public class SpawnManager2 : MonoBehaviour {

    #region Public Variables  
    public GameObject TrailAndBall;
    public float SpawnFrequency = 10f;
    public Text ScanText;
    #endregion
    
    #region Private Variables
    private int numPlane;
    private int spawnCount;
    private float timer = 0.0f;
    private bool init = false;
    private GameObject[] planes;
    #endregion
    
    #region Unity Methods
    private void Start() {
        
    }
    private void Update() {
        timer += Time.deltaTime;

        if (init) {
            if (timer > SpawnFrequency) { // spawn a ball every 10 seconds
                timer = 0.0f;
                spawnCount++;
                RandomSpawn();
                ScanText.text = "Spawned " + spawnCount.ToString();
            }
            
        }
    }
    #endregion
    
    #region Private Methods

    // first select a random plane
    // then randomly select one point on the plane to spawn the ball
    private void RandomSpawn() {
        DisplayTrailAndBall spawner = TrailAndBall.GetComponent<DisplayTrailAndBall>();

        int startIndex = Random.Range(0, numPlane);
        int endIndex = startIndex;
        while (startIndex == endIndex) endIndex = Random.Range(0, numPlane);

        Vector3 startLoc = GetRandomFromPlane(startIndex, spawner);
        Vector3 endLoc = GetRandomFromPlane(endIndex, spawner);

        spawner.start = startLoc;
        spawner.end = endLoc;
        spawner.middle = (startLoc + endLoc) / 2;
        
        Instantiate(TrailAndBall, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));


    }

    private Vector3 GetRandomPointAroundPlane(GameObject plane) {
        float x = Random.Range(plane.transform.position.x - plane.transform.localScale.x/2,
            plane.transform.position.x + plane.transform.localScale.x/2);
        float y = Random.Range(plane.transform.position.y - plane.transform.localScale.y/2,
            plane.transform.position.y + plane.transform.localScale.y/2);
        float z = Random.Range(plane.transform.position.z - plane.transform.localScale.z/2,
            plane.transform.position.z + plane.transform.localScale.z/2);

        return new Vector3(x, y, z);
    }

    private Vector3 GetRandomFromPlane(int index, DisplayTrailAndBall spawner) {
        // [0, n - 2) are walls, n - 2 is floor, n - 1 is ceiling
        // walls take linear algebra approach
        // 
        Vector3 loc;
        Collider coll;
        if (index < numPlane - 2) {
            PlayspaceWall wall = Playspace.Instance.Walls[index];
            float widthRandom = Random.Range(0f, 1f);
            float heightRandom = Random.Range(-0.5f, 0.5f);

            loc = widthRandom*(wall.RightEdge - wall.LeftEdge) + wall.LeftEdge;
            loc += Vector3.up*(wall.height*heightRandom);

            GameObject walls = Playspace.Instance.WallGeometry;
            coll = walls.GetComponent<Collider>();
        } else if (index < numPlane - 1) {
            GameObject floor = Playspace.Instance.FloorGeometry;
            Vector3 floorpoint = GetRandomPointAroundPlane(floor);

            coll = floor.GetComponent<Collider>();
            loc = coll.ClosestPointOnBounds(floorpoint);
        } else {
            GameObject ceiling = Playspace.Instance.CeilingGeometry;
            Vector3 ceilingpoint = GetRandomPointAroundPlane(ceiling);

            coll = ceiling.GetComponent<Collider>();
            loc = coll.ClosestPointOnBounds(ceilingpoint);
        }
        spawner.ball.GetComponent<Explosion>().startPlaneCollider = coll;


        return loc;
    }

    private void Awake() {
        //hooks:
        Playspace.Instance.OnCompleted.AddListener(HandleCompleted);
    }

    private void HandleCompleted() {
        init = true;
        numPlane = Playspace.Instance.Walls.Length + 2;
        spawnCount = 0;


    #endregion
    }
    }
}