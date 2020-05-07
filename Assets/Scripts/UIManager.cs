using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
public class UIManager : MonoBehaviour {

  #region Public Variables  
  public enum Mode {LOOSE, SOFT, HARD};
  public Mode WorldMode;
  public GameObject WorldCanvas;
  public Text scoreText;
  public Text summaryText;
  public GameObject Camera;
  public ScoreKeeping controllerScript;
  public GameObject ball;
  #endregion
  
  #region Private Variables
  private HeadLockScript _headlock;
  private const float _triggerThreshold = 0.2f;
  private const float _rotspeed = 10.0f;
  private bool _triggerPressed = false;
  private MLInput.Controller _control;
  private float timer = 0.0f;
  private int prevScore = 0;
  #endregion
  
  #region Unity Methods
  private void Start() {
    // Get the HeadLockScript script
    _headlock = GetComponentInChildren<HeadLockScript>();
    
    // Setup and Start Magic Leap input and add a button event (will be used for the HomeTap)
    MLInput.Start();
    MLInput.OnControllerButtonUp += OnButtonUp;
    _control = MLInput.GetController(MLInput.Hand.Left);
    
    // Reset the scene
    Reset();
  }
  private void Update() {
    timer += Time.deltaTime;

    // update score if changed
    if (controllerScript.score > prevScore) {
      SetScoreText();
      prevScore = controllerScript.score;
    }
    
    // Check and adjust for headlock status
    CheckStates();
    CheckControl();
  }
  private void OnDestroy () {
    MLInput.OnControllerButtonUp -= OnButtonUp;
    MLInput.Stop();
  }
  #endregion
  
  #region Private Methods
   /// CheckStates
   /// Switch headlock mode depending on the world mode
   ///
    private void CheckStates() {
      if (WorldMode == Mode.LOOSE) {
        _headlock.HeadLock(WorldCanvas, 1.75f);
      }
      else if (WorldMode == Mode.SOFT) {
        _headlock.HeadLock(WorldCanvas, 5.0f);
      }
      else {
        _headlock.HardHeadLock(WorldCanvas);
      }
    }
    /// CheckControl
   /// Monitor the trigger to spawn object
   ///
   private void CheckControl() {
     if (_control.TriggerValue > _triggerThreshold) {
       _triggerPressed = true;
     }
     else if (_control.TriggerValue == 0.0f && _triggerPressed) {
       _triggerPressed = false;
       // when trigger is pressed, spawn object
       Instantiate(ball, _control.Position, _control.Orientation);

       
     }
   }
   /// Reset
   /// Resets the scene back to the starting screen (score = 0)
   ///    
   private void Reset() {
     WorldMode = Mode.LOOSE;
     summaryText.text = "";

     // clear all the score count and timer
     prevScore = 0;
     controllerScript.score = 0;
     controllerScript.up = 0;
     controllerScript.medium = 0;
     controllerScript.down = 0;
     timer = 0.0f;
     SetScoreText();
   }

   /// OnButtonUp
   /// Button event - show summary when bumper  is tapped
   /// Button event - reset scene when home button is tapped
   private void OnButtonUp(byte controller_id, MLInput.Controller.Button button) {
     if (button == MLInput.Controller.Button.Bumper) {
       SetSummaryText();
     }
     if (button == MLInput.Controller.Button.HomeTap) {
       Reset();
     }
   }

   private void SetScoreText() {
     scoreText.text = "Score: " + controllerScript.score.ToString();
     
     // update summary if it's already shown on the screen
     if (summaryText.text != "") {
       SetSummaryText();
     }
   }

   private void SetSummaryText() {
     summaryText.text = "Summary:" + "\nScore: " + controllerScript.score.ToString() 
     + ";\nUp-stretch: " + controllerScript.up.ToString() + ";\nDown-squat: " + controllerScript.down.ToString()
     + ";\nCurrent Session Time: " + (timer/60).ToString("F1")
     + "mins;\nPress Home to start a new session";
   }
   #endregion
}
