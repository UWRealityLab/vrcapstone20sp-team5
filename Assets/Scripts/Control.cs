using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Control : MonoBehaviour {
   #region Public Variables 
   public enum ButtonStates { 
	   Normal, 
	   Pressed, 
	   JustReleased
   };
   public ButtonStates BtnState;  
   #endregion

   #region Private Variables
   private const float TIME_MESH_SCANNING_TOGGLE = 3.0f;
   private bool _held = false;
   private float _startTime = 0.0f;
   private MeshingScript _meshing;
   #endregion

   #region Unity Methods
   private void Start() {
       // Start input
       MLInput.Start();

       // Add button callbacks
       MLInput.OnControllerButtonDown += HandleOnButtonDown;
       MLInput.OnControllerButtonUp += HandleOnButtonUp;

       // Assign meshing component
       _meshing = GetComponent<MeshingScript>();

       // Initial State of the Control is Normal
       BtnState = ButtonStates.Normal;
   }

   private void OnDestroy() {
       // Stop input
       MLInput.Stop();

       // Remove button callbacks
       MLInput.OnControllerButtonDown -= HandleOnButtonDown;
       MLInput.OnControllerButtonUp -= HandleOnButtonUp;
   }

   private void Update() {
       // Bumper button held down - toggle scanning if timer reaches max
       if (GetTime() >= TIME_MESH_SCANNING_TOGGLE && BtnState == ButtonStates.Pressed) {
           _held = true;  
           _startTime = Time.time;
           _meshing.ToggleMeshScanning();               
       }
       // Bumper was just released - toggle visibility
       else if (BtnState == ButtonStates.JustReleased) {
             BtnState = ButtonStates.Normal;
             _startTime = 0.0f;
             if (!_held) {
                _meshing.ToggleMeshVisibility();         
             } 
             else {
                _held = false;
             }
       }
   }
   #endregion

   #region Private Methods
   public float GetTime()  {
       float returnTime = -1.0f;
       if (_startTime > 0.0f) {
           returnTime = Time.time - _startTime;
       }
       return returnTime;
   }
   #endregion

   #region Event Handlers
   void HandleOnButtonUp(byte controller_id, MLInput.Controller.Button button) {
       // Callback - Button Up
       if (button == MLInput.Controller.Button.Bumper) {
           BtnState = ButtonStates.JustReleased;
       }
   }

   void HandleOnButtonDown(byte controller_id, MLInput.Controller.Button button) {
       // Callback - Button Down
       if (button == MLInput.Controller.Button.Bumper) {
           // Start bumper timer
            _startTime = Time.time;
           BtnState = ButtonStates.Pressed;
       }
   }
   #endregion
}
