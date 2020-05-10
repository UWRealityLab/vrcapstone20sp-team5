using UnityEngine;
using MagicLeapTools;


public class SpawnManager : MonoBehaviour {

    #region Public Variables  
    public float SpawnFrequency;
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
                spawnCount++;
                RandomSpawn();
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

        spawner.setParams(startLoc, endLoc, startRot, endRot, (startLoc + endLoc) / 2);
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
        } else {
            GameObject ceiling = Playspace.Instance.CeilingGeometry;
            loc = GetRandomPointAroundPlane(ceiling);
            quat = ceiling.transform.rotation;
        }
        return (loc, quat);
    }

    private void HandleCompleted() {
        init = true;
        numPlane = Playspace.Instance.Walls.Length + 2;
        spawnCount = 0;
    }

    #endregion
}