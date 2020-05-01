using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class BumpButtonDown : MonoBehaviour
{

    public GameObject obj;
    private MLInput.Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        MLInput.Start();
        MLInput.OnControllerButtonDown += OnButtonDown;
        controller = MLInput.GetController(MLInput.Hand.Left);
    }

    void OnButtonDown(byte controller_id, MLInput.Controller.Button button) {
        Instantiate(obj, controller.Position, controller.Orientation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
