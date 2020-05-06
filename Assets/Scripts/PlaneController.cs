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
    public float ScanningTimeOut = 30f; 


    private MLPlanes.QueryParams _queryParams = new MLPlanes.QueryParams();
    public MLPlanes.QueryFlags QueryFlags;
    
    private float timeout = 5f;
    private float timeSinceLastRequest = 0f;
    private float totalTimeElapsed = 0f;
    HashSet<ulong> planeId = new HashSet<ulong>();
    public List<GameObject> planeCache = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        MLPlanes.Start();
        ScanText.text = "Scanning for walls/ceiling/floor ";
    }


    // Update is called once per frame
    void Update()
    {
        timeSinceLastRequest += Time.deltaTime;
        totalTimeElapsed += Time.deltaTime;
        
        // check if plane extraction is completed by
        // 1) checking if number of planes passes threshold
        // 2) checking if time spent passes ScanningTimeOut
        if (!Completed && (planeCache.Count >= Threshold || 
            totalTimeElapsed > ScanningTimeOut)) {
            Completed = true;
            MLPlanes.Stop();
            ScanText.text = "Scanning Completed";
        }
        

        // if plane extraction not completed,
        // update progress for the user
        if (!Completed) {
            ScanText.text = "Scanning for walls/ceiling/floor " + 
              (planeCache.Count*100f/Threshold).ToString("F1") + "%";

            // trigger more requrest
            if (timeSinceLastRequest > timeout)
            {
                timeSinceLastRequest = 0f;
                RequestPlanes();
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
        GameObject newPlane;
        for (int i = 0; i < planes.Length; ++i)
        {
            if (!planeId.Contains(planes[i].Id)) { // check duplicate planes
                planeId.Add(planes[i].Id);
                newPlane = Instantiate(PlaneGameObject);
                newPlane.transform.position = planes[i].Center;
                newPlane.transform.rotation = planes[i].Rotation;
                newPlane.transform.localScale = new Vector3(planes[i].Width, planes[i].Height, 1f); // Set plane scale
                newPlane.tag = "Collision";
                planeCache.Add(newPlane);
            }
            
        }
    }
}
