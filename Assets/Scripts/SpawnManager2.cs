using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using static System.Math;
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
    private void OnDestroy () {
        
    }
    #endregion
    
    #region Private Methods

    // first select a random plane
    // then randomly select one point on the plane to spawn the ball
    private void RandomSpawn() {

        int startIndex = Random.Range(0, numPlane);
        int endIndex = startIndex;
        while (startIndex == endIndex) endIndex = Random.Range(0, numPlane);

        PlayspaceWall planeStart = Playspace.Instance.Walls[startIndex];
        PlayspaceWall planeEnd = Playspace.Instance.Walls[endIndex];

        DisplayTrailAndBall spawner = TrailAndBall.GetComponent<DisplayTrailAndBall>();

        float startWidthRandom = Random.Range(0f, 1f);
        float startHeightRandom = Random.Range(-0.5f, 0.5f);

        // Center - Up * (height * .5f);
        // Center + Up * (height * .5f);

        Vector3 startW = startWidthRandom*(planeStart.RightEdge - planeStart.LeftEdge) + planeStart.LeftEdge;
        Vector3 startH = planeStart.Center + startHeightRandom*(Vector3.up*planeStart.height);
        Vector3 startLoc = startW + Vector3.up*(planeStart.height*startHeightRandom);
        
        
        float endWidthRandom = Random.Range(0f, 1f);
        float endHeightRandom = Random.Range(-0.5f, 0.5f);

        Vector3 endW = endWidthRandom*(planeEnd.RightEdge - planeEnd.LeftEdge) + planeEnd.LeftEdge;
        Vector3 endH = planeEnd.Center + endHeightRandom*(Vector3.up*planeEnd.height);
        Vector3 endLoc = endW + Vector3.up*(planeEnd.height*endHeightRandom);

        spawner.start = startLoc;
        spawner.end = endLoc;
        spawner.middle = (startLoc + endLoc) / 2;

        // // set the startPlane in explosion script 
        // // so ball would not explode at the starting point
        // spawner.ball.GetComponent<Explosion>().startPlane = planeStart;
        // spawner.ball.GetComponent<Explosion>().endPlane= planeEnd;
        
        Instantiate(TrailAndBall, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        
        
    }

    private void Awake()
    {
        //hooks:
        Playspace.Instance.OnCleared.AddListener(HandleCleared);
        Playspace.Instance.OnCompleted.AddListener(HandleCompleted);
    }

    private void HandleCleared()
    {
        // primaryWallPlaque.gameObject.SetActive(false);
        // rearWallPlaque.gameObject.SetActive(false);
        // rightWallPlaque.gameObject.SetActive(false);
        // leftWallPlaque.gameObject.SetActive(false);
        // ceilingPlaque.gameObject.SetActive(false);
        // floorPlaque.gameObject.SetActive(false);
        // centerPlaque.gameObject.SetActive(false);
    }

    private void HandleCompleted()
    {
        init = true;
        numPlane = Playspace.Instance.Walls.Length;
        spawnCount = 0;
    }


    #endregion
    }
}