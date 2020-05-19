using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class BeamController : MonoBehaviour
{
    public Color startColor;
    public Color endColor;
    public delegate void OnUIButtonPress(string tag);
    public event OnUIButtonPress OptionSelected;
    private LineRenderer line;
    private MLInput.Controller controller;
    private bool triggerDown;

    private void Awake() {
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        Debug.Log("line disabled");
    }

    private void OnEnable() {
        line.enabled = true;
        Debug.Log("line enabled");
        line.startColor = startColor;
        line.endColor = endColor;
        MLInput.Start();
        controller = MLInput.GetController(MLInput.Hand.Left);
        triggerDown = false;
    }

    private void OnDisable() {
        line.enabled = false;
        MLInput.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = gameObject.transform.position;
        transform.rotation = gameObject.transform.rotation;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit)) {
            line.useWorldSpace = true;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, hit.point);
            if (!triggerDown && controller.TriggerValue > 0.8f) {
                triggerDown = true;
                if (hit.transform.tag != null) {
                    OptionSelected?.Invoke(hit.transform.tag);
                }
            }

        } else {
            line.useWorldSpace = false;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, Vector3.forward * 5);
        }

        if (!triggerDown && controller.TriggerValue > 0.8f) triggerDown = true;
        if (controller.TriggerValue < 0.2f) triggerDown = false;
    }
}
