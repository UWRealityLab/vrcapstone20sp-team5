﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
public class UIManager : MonoBehaviour {

    #region Public Variables  
    public Text scoreText;
    public Text summaryText;
    public Text status;
    public Text spawnCountText;
    
    public ScoreKeeping scoreKeeper;
    #endregion
    
    #region Private Variables
    private MLInput.Controller _control;
    #endregion
    
    #region Unity Methods
    private void Start() {
        // Setup and Start Magic Leap input and add a button event (will be used for the HomeTap)
        MLInput.Start();
        MLInput.OnControllerButtonUp += OnButtonUp;
        _control = MLInput.GetController(MLInput.Hand.Left);
        
        // subscribe to score change event
        scoreKeeper.ScoreChange += OnScoreChange;
        
        summaryText.enabled = false;
    }

    private void OnDestroy () {
        MLInput.OnControllerButtonUp -= OnButtonUp;
        scoreKeeper.ScoreChange -= OnScoreChange;
        MLInput.Stop();
    }
    #endregion
    
    #region Private Methods

    /// OnButtonUp
    /// Button event - when home is tapped: show summary
    /// Button event - when home is tapped again: reset scene
    private void OnButtonUp(byte controller_id, MLInput.Controller.Button button) {
        if (button == MLInput.Controller.Button.HomeTap) {
            if (!summaryText.enabled) {
                summaryText.enabled = true;
            } else {
                summaryText.enabled = false;
                scoreKeeper.ResetScore();
            }
        }
    }

    private void OnScoreChange(int change, ScoreKeeping.ChangeType type) {
        switch (type) {
            case ScoreKeeping.ChangeType.Down:
                SetStatusText("down-squat +1!");
                break;
            case ScoreKeeping.ChangeType.Up:
                SetStatusText("up-strech +1!");
                break;
            case ScoreKeeping.ChangeType.Middle:
                SetStatusText("Good job!");
                break;
            case ScoreKeeping.ChangeType.Reset:
                SetStatusText("");
                break;
            default:
                break;
        }
        SetScoreText();
        SetSummaryText();
    }
    #endregion

    #region Public Methods
    public void SetStatusText(string text) {
        status.text = text;
    }

    public void SetScoreText() {
        scoreText.text = "Score: " + scoreKeeper.score.ToString();
    }

    public void SetSummaryText() {
        summaryText.text = "Summary:" + "\nScore: " + scoreKeeper.score.ToString() 
        + ";\nUp-stretch: " + scoreKeeper.up.ToString() + ";\nDown-squat: " + scoreKeeper.down.ToString()
        + ";\nCurrent Session Time: " + (scoreKeeper.timer/60).ToString("F1")
        + "mins;\nPress Home to start a new session";
    }

    public void SetSpawnCountText(int count) {
        spawnCountText.text = "Spawned: " + count.ToString();
    }
    #endregion
}
