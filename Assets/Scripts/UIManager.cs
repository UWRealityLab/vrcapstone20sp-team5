using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using MagicLeapTools;
using static System.Math;
using System;

public class UIManager : MonoBehaviour {

    #region Public Variables  
    public Text scoreText;
    public Text summaryText;
    public Text statusText;
    public Text helpText;
    public Text meshingPrompt; 
    public GameObject menu;
    public GameObject canvas;
    public GameObject settings;
    public BeamController beamController;   
    public ScoreKeeping scoreKeeper;
    public enum WallStat {Empty, Summary, Setting, Help, Menu};
    public enum SettingType {Freq, Speed, BGM, Sound, Path, Mesh, Height, GameMode, Duration};
    #endregion
    
    #region Private Variables
    private SpawnManager spawnMngr;
    private AudioManager audioManager;
    private ScoreKeeping scoreKeeping;
    private DisplayTrailAndBall trailAndBall;
    private MLInput.Controller _control;
    private WallStat wallStat;
    private bool pathOn;
    private bool meshOn;
    #endregion
    
    #region Unity Methods
    private void Awake() {
        MLInput.Start();
        spawnMngr = GetComponent<SpawnManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        scoreKeeping = GameObject.Find("[Content]/Controller").GetComponent<ScoreKeeping>();
        trailAndBall = spawnMngr.trailAndBall.GetComponent<DisplayTrailAndBall>();
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

        // update setting text to reflect current settings
        pathOn = trailAndBall.trails[0].activeSelf;
        if (GameObject.Find("[Content]/MLSpatialMapper/Original").GetComponent<Renderer>().material.mainTexture == null)
            meshOn = false;
        else meshOn = true;
        

        foreach(SettingType type in Enum.GetValues(typeof(SettingType))) 
            SetSettingText(type);

        Playspace.Instance.OnCompleted.AddListener(OnPlayspaceComplete);
        Playspace.Instance.OnCleared.AddListener(OnPlayspaceClear);
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
    private void OnPlayspaceClear() {
        meshingPrompt.enabled = false;
    }

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
        canvas.transform.position = wall.Center + Vector3.up * .3f;
        canvas.transform.rotation = Quaternion.LookRotation(wall.Back, Vector3.up);
        canvas.transform.Translate(Vector3.back * .1f); // pull the canvas up a bit to avoid coverup
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
        // setting page interactions
        if (tag == "freq_plus") {
            // frequency convertion: 
            // frequency level 1: 10 sec/spawn, lv2: 5 sec/spawn, lv3: 2.5 sec/spawn...
            // see SpawnManager.spawnInterval for more
            spawnMngr.SpawnFrequency = Min(spawnMngr.SpawnFrequency + 1, 7);
            SetSettingText(SettingType.Freq);
        } else if (tag == "freq_minus") {
            spawnMngr.SpawnFrequency = Max(spawnMngr.SpawnFrequency - 1, 1);
            SetSettingText(SettingType.Freq);
        } else if (tag == "BGM_plus") {
            audioManager.background.volume = Min(audioManager.background.volume + 0.1f, 1f);
            SetSettingText(SettingType.BGM);
        } else if (tag == "BGM_minus") {
            audioManager.background.volume = Max(audioManager.background.volume - 0.1f, 0f);
            SetSettingText(SettingType.BGM);
        } else if (tag == "sound_plus") {
            float newValue = Min(audioManager.grab.volume + 0.1f, 1f);
            audioManager.grab.volume = newValue;
            audioManager.crash.volume = newValue;
            audioManager.spawn.volume = newValue;
            
            SetSettingText(SettingType.Sound);
        } else if (tag == "sound_minus") {
            float newValue = Max(audioManager.grab.volume - 0.1f, 0f);
            audioManager.grab.volume = newValue;
            audioManager.crash.volume = newValue;
            audioManager.spawn.volume = newValue;

            SetSettingText(SettingType.Sound);
        } else if (tag == "path_control") {
            GameObject[] trails = spawnMngr.trailAndBall.GetComponent<DisplayTrailAndBall>().trails;
            if (pathOn) {
                foreach(GameObject obj in trails) obj.SetActive(false);
                pathOn = false;
            } else {
                foreach(GameObject obj in trails) obj.SetActive(true);
                pathOn = true;
            }
            SetSettingText(SettingType.Path);
        } else if (tag == "height_plus") {
            scoreKeeping.upLimit = Min(scoreKeeping.upLimit + 0.01f, 3f);
            SetSettingText(SettingType.Height);
        } else if (tag == "height_minus") {
            scoreKeeping.upLimit = Max(scoreKeeping.upLimit - 0.01f, 0f);
            SetSettingText(SettingType.Height);
        } else if (tag == "speed_plus") {
            trailAndBall.speed =  Min(trailAndBall.speed + 0.1f, 3f);
            SetSettingText(SettingType.Speed);
        } else if (tag == "speed_minus") {
            trailAndBall.speed =  Max(trailAndBall.speed - 0.1f, 0f);
            SetSettingText(SettingType.Speed);
        } else if (tag == "mesh_control") {
            if (meshOn) {
                GameObject.Find("[Content]/MLSpatialMapper/Original").GetComponent<Renderer>().material.mainTexture = null;
                meshOn = false;
            } else {
                Texture meshTexture = Resources.Load<Texture>("GradientLine");
                GameObject.Find("[Content]/MLSpatialMapper/Original").GetComponent<Renderer>().material.mainTexture = meshTexture;
                meshOn = true;
            }
            SetSettingText(SettingType.Mesh);
        } else if (tag == "playspace_reset") {
            settings.SetActive(false);
            beamController.enabled = false;
            Playspace.Instance.Rebuild();
        } else if (tag == "gamemode") {
            if (spawnMngr.timedMode) {
                spawnMngr.timedMode = false;
                spawnMngr.survivalMode = true;
            }
            else if (spawnMngr.survivalMode) {
                spawnMngr.survivalMode = false;
            } else {
                spawnMngr.timedMode = true;
            }
            //spawnMngr.timedMode = !spawnMngr.timedMode;
            SetSettingText(SettingType.GameMode);
        } else if (tag == "duration_plus") {
            spawnMngr.timeLimit += 30;
            SetSettingText(SettingType.Duration);
        } else if (tag == "duration_minus") {
            if (spawnMngr.timeLimit >= 60) {
                spawnMngr.timeLimit -= 30;
            }
            SetSettingText(SettingType.Duration);
        }
        Debug.Log("Option selected" + tag);
    }

    private void SetSettingText(SettingType type) {
        Text value;
        if (type == SettingType.Freq) {
            value = settings.transform.Find("FreqControl/TextField/Value").GetComponent<UnityEngine.UI.Text>();
            value.text = spawnMngr.SpawnFrequency.ToString("F0");
        } else if (type == SettingType.BGM) {
            value = settings.transform.Find("BGMControl/TextField/Value").GetComponent<UnityEngine.UI.Text>();
            value.text =  (audioManager.background.volume*100).ToString("F0");
        } else if (type == SettingType.Sound) {
            value = settings.transform.Find("SoundEffectControl/TextField/Value").GetComponent<UnityEngine.UI.Text>();
            value.text =  (audioManager.grab.volume*100).ToString("F0");
        } else if (type == SettingType.Path) {
            value = settings.transform.Find("BallPathControl/TextField/Value").GetComponent<UnityEngine.UI.Text>();
            value.text =  pathOn ? "ON" : "OFF";
        } else if (type == SettingType.Height) {
            value = settings.transform.Find("HeightControl/TextField/Value").GetComponent<UnityEngine.UI.Text>();
            value.text =  (scoreKeeping.upLimit*100).ToString("F0");
        } else if (type == SettingType.Speed) {
            value = settings.transform.Find("SpeedControl/TextField/Value").GetComponent<UnityEngine.UI.Text>();
            value.text =  (trailAndBall.speed).ToString("F1");
        } else if (type == SettingType.Mesh) {
            value = settings.transform.Find("MeshVisualControl/TextField/Value").GetComponent<UnityEngine.UI.Text>();
            value.text =  meshOn ? "ON" : "OFF";
        } else if (type == SettingType.GameMode) {
            value = settings.transform.Find("GameMode/TextField/Value").GetComponent<UnityEngine.UI.Text>();
            if (spawnMngr.timedMode) {
                value.text = "Timed";
            } else if (spawnMngr.survivalMode) {
                value.text = "Survival";
            } else {
                value.text = "Unlimited";
            }
            //value.text = spawnMngr.timedMode ? "Timed" : "Unlimited";
        } else if (type == SettingType.Duration) {
            value = settings.transform.Find("GameDuration/TextField/Value").GetComponent<UnityEngine.UI.Text>();
            value.text = (spawnMngr.timeLimit / 60f).ToString("F1");
        }
    }

    private void OnScoreChange(int change, ScoreKeeping.ChangeType type) {
        switch (type) {
            case ScoreKeeping.ChangeType.Down:
                SetStatusText("Squat +1!");
                break;
            case ScoreKeeping.ChangeType.Up:
                SetStatusText("Stretch +1!");
                break;
            case ScoreKeeping.ChangeType.Middle:
                SetStatusText("Good job!");
                break;
            case ScoreKeeping.ChangeType.Reset:
                SetStatusText("");
                spawnMngr.timeLeft = spawnMngr.timeLimit;
                break;
            default:
                break;
        }
        if (type != ScoreKeeping.ChangeType.Reset) {
            _control.StartFeedbackPatternVibe(MLInput.Controller.FeedbackPatternVibe.Buzz, MLInput.Controller.FeedbackIntensity.Medium);
        }
        SetScoreText();
        SetSummaryText(spawnMngr.timedMode, spawnMngr.survivalMode, spawnMngr.timeLeft);
    }
    #endregion

    #region Public Methods
    public string setStringBlue(string text) {
        return "<color=#2e9ad9ff>" + text + "</color>";
    }

    public string setStringOrange(string text) {
        return "<color=#f28f2cff>" + text + "</color>";
    }

    public void SetStatusText(string text) {
        statusText.text = text;
    }

    public void SetScoreText() {
        scoreText.text = "Score: " + setStringBlue(scoreKeeper.score.ToString());
    }

    public void SetSummaryText(bool timed, bool survival, float timeLeft) {
        if (timed && timeLeft > 0) {
            summaryText.fontSize = 10;
            summaryText.alignment = TextAnchor.MiddleCenter;
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeLeft);
            summaryText.text =  "<size=50>"+ setStringOrange(TimeSpan.FromSeconds(timeLeft).ToString(@"mm\:ss"))+ "</size>"+
                "\n Balls Spawned: " + setStringOrange(scoreKeeper.spawnCount.ToString())+ "\n"
                + "\n Stretches: " + setStringOrange(scoreKeeper.up.ToString()) + "\n"
                + "Squats: " + setStringOrange(scoreKeeper.down.ToString()) + "\n\n" 
                + setStringBlue("\nPress BUMPER to hide the summary\n"
                + "Press HOME to return to the menu");
        } else if (!survival) {
            summaryText.fontSize = 10;
            summaryText.alignment = TextAnchor.MiddleCenter;
            summaryText.text = 
                "\n Balls Spawned: " + setStringOrange(scoreKeeper.spawnCount.ToString())+ "\n"
                + "\n Stretches: " + setStringOrange(scoreKeeper.up.ToString()) + "\n"
                + "Squats: " + setStringOrange(scoreKeeper.down.ToString()) + "\n\n"
                + (timed ? ("Session Time: " + (spawnMngr.timeLimit/60).ToString("F1") + " minutes\n") : 
                    ("Current Session Time: " + (scoreKeeper.timer/60).ToString("F1") + " minutes\n"))
                + setStringBlue("\nPress BUMPER to hide the summary\n"
                + "Press HOME to return to the menu");
        }
        else if (scoreKeeper.lives <= 0) {
            summaryText.fontSize = 10;
            summaryText.alignment = TextAnchor.MiddleCenter;
            summaryText.text = "<size=50>Game Over:</size>\n" +
                "\n Balls Spawned: " + setStringOrange(scoreKeeper.spawnCount.ToString())+ "\n"
                + "\n Stretches: " + setStringOrange(scoreKeeper.up.ToString()) + "\n"
                + "Squats: " + setStringOrange(scoreKeeper.down.ToString()) + "\n\n"  
                + setStringBlue("\nPress BUMPER to hide the summary\n"
                + "Press HOME to return to the menu");
        } else {
            summaryText.fontSize = 10;
            summaryText.alignment = TextAnchor.MiddleCenter;
            summaryText.text = 
            //+ "Spawned: " + scoreKeeper.spawnCount.ToString() + ";\n"
                "\n Balls Spawned: " + setStringOrange(scoreKeeper.spawnCount.ToString())+ "\n"
                + "\n Stretches: " + setStringOrange(scoreKeeper.up.ToString()) + "\n"
                + "Squats: " + setStringOrange(scoreKeeper.down.ToString()) + "\n\n" 
                + "Lives: " + setStringOrange(scoreKeeper.lives.ToString()) + "\n" 
            //+ "Current Session Time: " + (scoreKeeper.timer/60).ToString("F1") + "mins;\n"
            + setStringBlue("\nPress BUMPER to hide the summary\n"
            + "Press HOME to return to the menu");
        }
    }

    public void SetHelpText() {
        
        helpText.text = 
            "Press PLAY to start a game. \n" +
            "\n Change the game mode and set user preferences in GAME SETTINGS. \n" +
            "\n The goal of the game is to catch as many balls \n" +
            "as possible while avoiding obstacles. \n" +
            "You will earn +10 points per ball caught, good luck! \n" +
            "\n Press BUMPER to go back";
    }
    #endregion
}
