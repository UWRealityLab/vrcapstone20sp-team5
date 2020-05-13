using UnityEngine;
using MagicLeapTools;


public class SpawnManager : MonoBehaviour {

    #region Public Variables  
    public float SpawnFrequency;
    public float middlePointRange;
    public float angleMin;
    public float angleMax;
    public GameObject trailAndBall;
    #endregion
    
    #region Private Variables
    private UIManager UIMnger;
    private int numPlane;
    private int spawnCount;
    private float timer;
    private bool init = false;
    private GameObject[] planes;
    #endregion
    
    #region Unity Methods
    private void Update() {
        timer += Time.deltaTime;

        if (init) {
            if (timer > SpawnFrequency) {
                timer = 0.0f;
                RandomSpawn();
                spawnCount++;
                UIMnger.SetSpawnCountText(spawnCount);
            }
        }
    }

    private void Awake() {
        //hooks:
        Playspace.Instance.OnCompleted.AddListener(HandleCompleted);
        UIMnger = GetComponent<UIManager>();
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
               Vector3.Angle(middleLoc - endLoc, endRot * Vector3.forward) > 90) {
            middleLoc = (startLoc + endLoc) / 2 + 
                new Vector3(Random.Range(-middlePointRange, middlePointRange), 
                        Random.Range(-middlePointRange, middlePointRange),
                        Random.Range(-middlePointRange, middlePointRange));
            if (count++ > 50) break;
        }

        spawner.setParams(startLoc, endLoc, startRot, endRot, middleLoc);
        Instantiate(trailAndBall, Vector3.zero, Quaternion.identity);
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

    private (Vector3, Quaternion) GetRandomFromPlane(int index) {
        // [0, n - 2) are walls, n - 2 is floor, n - 1 is ceiling
        // walls take a linear algebra approach
        // ceiling/floor takes the closest point on collider bound
        Vector3 loc;
        Quaternion quat;
        Collider coll;
        if (index < numPlane - 2) {
            PlayspaceWall wall = Playspace.Instance.Walls[index];
            float widthRandom = Random.Range(0f, 1f);
            float heightRandom = Random.Range(-0.5f, 0.5f);

            loc = widthRandom*(wall.RightEdge - wall.LeftEdge) + wall.LeftEdge;
            loc += Vector3.up*(wall.height*heightRandom);
            quat = wall.Rotation;
        } else if (index < numPlane - 1) {
            GameObject floor = Playspace.Instance.FloorGeometry;
            loc = GetRandomPointAroundPlane(floor);
            quat = floor.transform.rotation;

            coll = floor.GetComponent<Collider>();
            loc = coll.ClosestPointOnBounds(loc);
        } else {
            GameObject ceiling = Playspace.Instance.CeilingGeometry;
            loc = GetRandomPointAroundPlane(ceiling);
            quat = ceiling.transform.rotation;

            coll = ceiling.GetComponent<Collider>();
            loc = coll.ClosestPointOnBounds(loc);
        }
        return (loc, quat);
    }

    private void HandleCompleted() {
        init = true;
        numPlane = Playspace.Instance.Walls.Length + 2;
        spawnCount = 0;

        Instantiate(GameObject.Find("AudioManager").GetComponent<AudioManager>().background,
                Playspace.Instance.Center,Quaternion.identity);
    }

    #endregion
}