using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

public class PlaneController : MonoBehaviour
{

    public Transform BBoxTransform; // Determines center of region for plane extraction
    public Vector3 BBoxExtents;     // Determines size of BBoxTransform region
                                    // (0, 0, 0) = boundless
    public GameObject PlaneGameObject;
    public Text ScanText;
    public int Threshold = 20;
    public bool Completed = false;


    private MLPlanes.QueryParams _queryParams = new MLPlanes.QueryParams();
    public MLPlanes.QueryFlags QueryFlags;
    
    private float timeout = 5f;
    private float timeSinceLastRequest = 0f;
    HashSet<ulong> planeId = new HashSet<ulong>();
    public List<GameObject> planeCache = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        MLPlanes.Start();
        ScanText.text = "Scanning for walls/ceiling/floor";
        // RequestPlanes();
    }

    private void OnDestroy()
    {
        // MLPlanes.Stop(); // Stop extracting planes when application exits

    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastRequest += Time.deltaTime;
        if (timeSinceLastRequest > timeout && !Completed)
        {
            timeSinceLastRequest = 0f;
            RequestPlanes();

            if (planeCache.Count >= Threshold) {
                Completed = true;
                MLPlanes.Stop();
                ScanText.text = "Scanning Completed";
            }
        }
    }

    void RequestPlanes()
    {
        _queryParams.Flags          = QueryFlags;
        _queryParams.MaxResults     = 100;
        _queryParams.BoundsCenter   = BBoxTransform.position;
        _queryParams.BoundsRotation = BBoxTransform.rotation;
        _queryParams.BoundsExtents  = BBoxExtents;

        MLPlanes.GetPlanes(_queryParams, HandleOnReceivedPlanes);
    }

    private void HandleOnReceivedPlanes(MLResult result, MLPlanes.Plane[] planes, MLPlanes.Boundaries[] boundaries)
    {
        // for (int i=planeCache.Count-1; i>=0; --i)
        // {
        //     Destroy(planeCache[i]);
        //     planeCache.Remove(planeCache[i]);
        // }

        GameObject newPlane;
        for (int i = 0; i < planes.Length; ++i)
        {
            if (!planeId.Contains(planes[i].Id)) { // check duplicate planes
                planeId.Add(planes[i].Id);
                newPlane = Instantiate(PlaneGameObject);
                newPlane.transform.position = planes[i].Center;
                newPlane.transform.rotation = planes[i].Rotation;
                newPlane.transform.localScale = new Vector3(planes[i].Width, planes[i].Height, 1f); // Set plane scale
                planeCache.Add(newPlane);
            }
            
        }

        // foreach (GameObject plane in planeCache)
        // {
        //     // Debug.Log("Added TimedSpawn script!");
        //     plane.AddComponent<TimedSpawn>();
        // }
    }
}
