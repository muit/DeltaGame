using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    //Settings
    [Header("Settings")]
    public bool adsEnabled = true;

    //Prefabs
    [Header("Prefabs")]
    public GameObject onlinePlayer;
    public GameObject offlinePlayer;
    [Space(10)]

    //References
    [Header("References")]
    public Camera playerCamera;
    [System.NonSerialized]
    public CPlayer controlledPlayer;
    public Spawn activeSpawn;
    public List<Spawn> spawns;
    public Scene scene;
    public TOD_Time skyTime;
    public ShowAd ads;

    public Text hudMeters;
    public Text hudBestMark;
    public Text pauseBestMark;
    public InputField playerName;

    [System.NonSerialized]
    public InControl.InControlManager inControl;
    [System.NonSerialized]
    public InControl.TouchManager inControlTouch;


    private static int lastBestMark = 0;


    void Awake()
    {
        inControl = FindObjectOfType<InControl.InControlManager>();
        inControlTouch = FindObjectOfType<InControl.TouchManager>();
    }

    public enum GameState {
        PLAYING,
        PAUSE,
        STOP
    }

    private static Game game;

    public static Game Get(){
        if(game){
            return game;
        }
        else{
            game = FindObjectOfType<Game>();
            return game;
        }
    }

    private static SaveLoad cache;

    public static SaveLoad GetCache()
    {
        if (cache)
        {
            return cache;
        }
        else
        {
            cache = FindObjectOfType<SaveLoad>();
            return cache;
        }
    }

    private static GUIManager guiManager;

    public static GUIManager GetGUI()
    {
        if (guiManager)
        {
            return guiManager;
        }
        else
        {
            guiManager = FindObjectOfType<GUIManager>();
            return guiManager;
        }
    }

    void Start() {
        StartGame();
    }

    void Update() {
        if (Input.GetKeyUp("escape")) {
            if (Game.state == GameState.PLAYING)
            {
                PauseGame();
            } else if (Game.state == GameState.PAUSE) {
                StopGame();
            }
        }
    }

    //Static Game
    public static GameState state = GameState.PAUSE;

    public static void StartGame(){
        state = GameState.PLAYING;
        GetGUI().Hide("PauseMenu");
        GetGUI().Show("UI");

        Game game = Get();
            
        if (game.controlledPlayer) {
            game.controlledPlayer.Respawn();
        }

        game.inControlTouch.controlsEnabled = true;

        game.hudBestMark.text = game.pauseBestMark.text;
        lastBestMark = int.Parse(game.pauseBestMark.text);
        
        game.skyTime.Reset();
    }

    public static void PauseGame() {
        state = GameState.PAUSE;


        GetGUI().Hide("UI");
        GetGUI().Show("PauseMenu");

        Game game = Get();
        game.controlledPlayer.Respawn(false);
        game.inControlTouch.controlsEnabled = false;

        game.pauseBestMark.text = game.hudBestMark.text;
        int actualBestMark = int.Parse(game.hudBestMark.text);
        if (lastBestMark < actualBestMark && Game.GetCache()) {
            //Throw name selection
            Game.GetCache().SaveBestMark(game.scene.seed, game.playerName.text, actualBestMark);
        }
        //Show 1 of each 4 times an ad
        if (game.adsEnabled && Random.Range(0f, 4f) > 3f) {
            game.ads.Show();
        }
    }

    public static void StopGame() {
        state = GameState.STOP;
        //GetGUI().Hide("UI");
        //GetGUI().Hide("PauseMenu");
        if(TNManager.isConnected)
            TNManager.Disconnect();
        else
            Application.LoadLevel("Menu");
    }

    public static bool IsMobile(){
        return (Application.platform == RuntimePlatform.Android
             || Application.platform == RuntimePlatform.IPhonePlayer);
    }

    public Transform GetTarget() {
        return (controlledPlayer != null)? controlledPlayer.transform : activeSpawn.transform;
    }
}
