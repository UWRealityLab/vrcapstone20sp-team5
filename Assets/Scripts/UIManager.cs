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
  private MLInput.Controller _control;
  private float _timer = 0.0f;
  private int _prevScore = 0;
  private bool _summaryOn = false;
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
    _timer += Time.deltaTime;

    // update score if changed
    if (controllerScript.score > _prevScore) {
      SetScoreText();
      _prevScore = controllerScript.score;
    }
    
    // Check and adjust for headlock status
    CheckStates();
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

   /// Reset
   /// Resets the scene back to the starting screen (score = 0)
   ///    
   private void Reset() {
     WorldMode = Mode.LOOSE;
     summaryText.text = "";

     // clear all the score count and timer
     _prevScore = 0;
     controllerScript.score = 0;
     controllerScript.up = 0;
     controllerScript.medium = 0;
     controllerScript.down = 0;
     _timer = 0.0f;
     SetScoreText();
   }

   /// OnButtonUp
   /// Button event - when home is tapped: show summary
   /// Button event - when home is tapped again: reset scene
   private void OnButtonUp(byte controller_id, MLInput.Controller.Button button) {
     if (button == MLInput.Controller.Button.HomeTap) {
       if (!_summaryOn) {
         _summaryOn = true;
         SetSummaryText();
       } else {
         _summaryOn = false;
         Reset();
       }
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
     + ";\nCurrent Session Time: " + (_timer/60).ToString("F1")
     + "mins;\nPress Home to start a new session";
   }
   #endregion
}
