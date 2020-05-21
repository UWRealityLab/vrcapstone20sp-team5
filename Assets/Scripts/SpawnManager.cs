using UnityEngine;
using MagicLeapTools;
using static System.Math;


public class SpawnManager : MonoBehaviour {

    #region Public Variables  
    public float SpawnFrequency;
    public float middlePointRange;
    public float angleMin;
    public float angleMax;
    public GameObject trailAndBall;
    public ScoreKeeping scorekeeper;
    #endregion
    
    #region Private Variables
    private UIManager UIMnger;
    private int numPlane;
    private float timer;
    private GameObject[] planes;
    private float xmin, xmax, zmin, zmax;
    private AudioSource sound;
    #endregion
    
    #region Unity Methods
    private void Update() {
        timer += Time.deltaTime;
        if (timer > SpawnFrequency) {
            timer = 0.0f;
            RandomSpawn();
            scorekeeper.spawnCount++;
            UIMnger.SetSummaryText();
        }
    }

    private void OnEnable() {
        timer = 0.0f;
        numPlane = Playspace.Instance.Walls.Length + 2;
        UIMnger = GetComponent<UIManager>();

        // iterate thourgh the wall to get a sense of the 2d shape
        xmin = Playspace.Instance.Walls[0].RightEdge.x;
        xmax = xmin;
        zmin = Playspace.Instance.Walls[0].RightEdge.z;
        zmax = zmin;

        for (int i = 1; i < Playspace.Instance.Walls.Length; i++) {
            xmin = Min(xmin, Playspace.Instance.Walls[i].RightEdge.x);
            xmax = Max(xmax, Playspace.Instance.Walls[i].RightEdge.x);

            zmin = Min(zmin, Playspace.Instance.Walls[i].RightEdge.z);
            zmax = Max(zmax, Playspace.Instance.Walls[i].RightEdge.z);

        }

        sound = Instantiate(GameObject.Find("AudioManager").GetComponent<AudioManager>().background,
                Playspace.Instance.Center,Quaternion.identity);
    }

    private void OnDisable() {
        Destroy(sound);
    }
    #endregion
    
    #region Private Methods

    // first select a random plane
    // then randomly select one point on the plane to spawn the ball
    private void RandomSpawn() {
        DisplayTrailAndBall spawner = trailAndBall.GetComponent<DisplayTrailAndBall>();

        int startIndex = Random.Range(0, numPlane);
        int endIndex = startIndex;
        while (startIndex == endIndex) endIndex = Random.Range(0, numPlane);

        (Vector3 startLoc, Quaternion startRot) = GetRandomFromPlane(startIndex);
        (Vector3 endLoc, Quaternion endRot) = GetRandomFromPlane(endIndex);

        // make sure the whole path is not too high (both end higher than 175cm)
        if (startLoc.y - Playspace.Instance.FloorCenter.y > 1.75f && 
            endLoc.y - Playspace.Instance.FloorCenter.y > 1.75f) {

            endIndex = startIndex;
            while (startIndex == endIndex) endIndex = Random.Range(0, numPlane);
            (endLoc, endRot) = GetRandomFromPlane(endIndex);
        }

        float angle = Quaternion.Angle(startRot, endRot);
        while (angle < angleMin || angle > angleMax) {
            endIndex = startIndex;
            while (startIndex == endIndex) endIndex = Random.Range(0, numPlane);
            (endLoc, endRot) = GetRandomFromPlane(endIndex); 
            angle = Quaternion.Angle(startRot, endRot);
        }

        Vector3 middleLoc = (startLoc + endLoc) / 2 + 
            new Vector3(Random.Range(-middlePointRange, middlePointRange), 
                        Random.Range(-middlePointRange, middlePointRange),
                        Random.Range(-middlePointRange, middlePointRange));
        int count = 0;
        while (Vector3.Angle(middleLoc - startLoc, startRot * Vector3.forward) > 90 ||
               Vector3.Angle(middleLoc - endLoc, endRot * Vector3.forward) > 90 ||
               !Playspace.Instance.Inside(middleLoc)) { // make sure path go into the space
            middleLoc = (startLoc + endLoc) / 2 + 
                new Vector3(Random.Range(-middlePointRange, middlePointRange), 
                        Random.Range(-middlePointRange, middlePointRange),
                        Random.Range(-middlePointRange, middlePointRange));
            if (count++ > 50) break;
        }

        spawner.setParams(startLoc, endLoc, startRot, endRot, middleLoc);
        Instantiate(trailAndBall, Vector3.zero, Quaternion.identity);
    }

    private Vector3 GetRandomPointAroundPlane() {
        float x = Random.Range(xmin, xmax);
        float y = Playspace.Instance.Center.y;
        float z = Random.Range(zmin, zmax);

        return new Vector3(x, y, z);
    }

    private (Vector3, Quaternion) GetRandomFromPlane(int index) {
        // [0, n - 2) are walls, n - 2 is floor, n - 1 is ceiling
        // walls take a linear algebra approach
        // ceiling/floor takes the closest point on collider bound
        Vector3 loc;
        Quaternion quat;
        bool inside = false;
        if (index < numPlane - 2) {
            PlayspaceWall wall = Playspace.Instance.Walls[index];
            float widthRandom = Random.Range(0f, 1f);
            float heightRandom = Random.Range(-0.5f, 0.5f);

            loc = widthRandom*(wall.RightEdge - wall.LeftEdge) + wall.LeftEdge;
            loc += Vector3.up*(wall.height*heightRandom);
            quat = wall.Rotation;
        } else if (index < numPlane - 1) {
            loc = Playspace.Instance.Center;
            while (!inside) {
                loc = GetRandomPointAroundPlane();
                inside = Playspace.Instance.Inside(loc);
            }
            loc.y = Playspace.Instance.FloorCenter.y;
            quat = Quaternion.Euler(90, 0, 0);
        } else {
            loc = Playspace.Instance.Center;
            while (!inside) {
                loc = GetRandomPointAroundPlane();
                inside = Playspace.Instance.Inside(loc);
            }
            
            loc.y = Playspace.Instance.CeilingCenter.y;
            quat = Quaternion.Euler(270, 0, 0);
        }
        return (loc, quat);
    }
    #endregion
    public void ChangeSpawnFrequency(float frequency)
    {  
        SpawnFrequency = frequency;
    }
}