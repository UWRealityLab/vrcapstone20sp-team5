using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using MagicLeapTools;

public class UIManager : MonoBehaviour {

    #region Public Variables  
    public Text scoreText;
    public Text summaryText;
    public Text statusText;
    public Text helpText;
    public GameObject menu;
    public GameObject canvas;
    public GameObject settings;
    public BeamController beamController;   
    public ScoreKeeping scoreKeeper;
    public enum WallStat {Empty, Summary, Setting, Help, Menu};
    #endregion
    
    #region Private Variables
    private SpawnManager spawnMngr;
    private MLInput.Controller _control;
    private WallStat wallStat;
    #endregion
    
    #region Unity Methods
    private void Awake() {
        MLInput.Start();
        spawnMngr = GetComponent<SpawnManager>();
        _control = MLInput.GetController(MLInput.Hand.Left);

        scoreText.enabled = false;
        summaryText.enabled = false;
        statusText.enabled = false;
        helpText.enabled = false;
        beamController.enabled = false;
        spawnMngr.enabled = false;
        menu.SetActive(false);
        canvas.SetActive(false);
        settings.SetActive(false);

        Playspace.Instance.OnCompleted.AddListener(OnPlayspaceComplete);
        MLInput.OnControllerButtonUp += OnButtonUp;
        scoreKeeper.ScoreChange += OnScoreChange;
        beamController.OptionSelected += OnOptionSelected;
        
        wallStat = WallStat.Empty;
    }

    private void OnDestroy () {
        MLInput.OnControllerButtonUp -= OnButtonUp;
        scoreKeeper.ScoreChange -= OnScoreChange;
        beamController.OptionSelected -= OnOptionSelected;
        MLInput.Stop();
    }
    #endregion
    
    #region Private Methods

    /// OnButtonUp
    /// Button event - when bumper is tapped: show summary
    /// Button event - when bumper is tapped again: reset scene
    private void OnButtonUp(byte controller_id, MLInput.Controller.Button button) {
        if (wallStat == WallStat.Summary) {
            if (button == MLInput.Controller.Button.Bumper) {
                scoreKeeper.ResetScore();
            } else if (button == MLInput.Controller.Button.HomeTap) {
                summaryText.enabled = false;
                spawnMngr.enabled = false;
                scoreText.enabled = false;
                statusText.enabled = false;
                beamController.enabled = true;
                menu.SetActive(true);
                wallStat = WallStat.Menu;
            }
        } else if (wallStat != WallStat.Empty) {  // in settings or help page
            settings.SetActive(false);
            helpText.enabled = false;
            wallStat = WallStat.Menu;
            menu.SetActive(true);
        }
    }

    private void OnPlayspaceComplete() {
        canvas.SetActive(true);
        menu.SetActive(true);
        PlayspaceWall wall = Playspace.Instance.Walls[Playspace.Instance.PrimaryWall];
        canvas.transform.position = wall.Center;
        canvas.transform.rotation = Quaternion.LookRotation(wall.Back, Vector3.up);
        //SetPositionAndRotation(wall.Center, wall.Rotation);
        beamController.enabled = true;
        wallStat = WallStat.Menu;
    }

    private void OnOptionSelected(string tag) {
        // only called when in menu page
        if (tag == "play") {
            menu.SetActive(false);
            beamController.enabled = false;
            wallStat = WallStat.Summary;
            spawnMngr.enabled = true;
            scoreKeeper.ResetScore();
            scoreText.enabled = true;
            summaryText.enabled = true;
            statusText.enabled = true;
        } else if (tag == "settings") {
            menu.SetActive(false);
            wallStat = WallStat.Setting;
            settings.SetActive(true);
        } else if (tag == "help") {
            menu.SetActive(false);
            wallStat = WallStat.Help;
            helpText.enabled = true;
            SetHelpText();
        }
        Debug.Log("Opeion selected" + tag);
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
        statusText.text = text;
    }

    public void SetScoreText() {
        scoreText.text = "Score: " + scoreKeeper.score.ToString();
    }

    public void SetSummaryText() {
        summaryText.text = "Summary:\n" 
        + "Spawned: " + scoreKeeper.spawnCount.ToString() + ";\n"
        + "Score: " + scoreKeeper.score.ToString() + ";\n" 
        + "Up-stretch: " + scoreKeeper.up.ToString() + ";\n"
        + "Down-squat: " + scoreKeeper.down.ToString() + ";\n" 
        + "Current Session Time: " + (scoreKeeper.timer/60).ToString("F1") + "mins;\n"
        + "Press Bumper to start a new session";
    }

    public void SetHelpText() {
        helpText.text = "No bb, catch the ball. In game tape home to return to main menu, press bumper to reset score.";
    }
    #endregion
}
