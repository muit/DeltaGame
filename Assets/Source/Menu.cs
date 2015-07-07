using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Menu : MonoBehaviour {
    [Header("References")]
    public InputField seedInput;
    public Button joinButton;
    public ScrollList bestMarks;
    public SaveLoad cache;

    private TNManager tnManager;

	void Start () {
        tnManager = FindObjectOfType<TNManager>();
        if (!bestMarks) {
            bestMarks = FindObjectOfType<ScrollList>();
        }
        bestMarks.PopulateList();
	}


	
	void Update () {
	}

    public void ShowSeed() {
        seedInput.text = cache.LoadSeed();
        //Load marks
        bestMarks.PopulateList(cache.LoadBestMark(seedInput.text));
    }
    public void SaveSeed() {
        cache.SaveSeed(seedInput.text);
        //Load marks
        cache.LoadBestMark(seedInput.text);
    }

    public void SetJoinButtonError(bool value) {
        joinButton.image.color = (value) ? new Color(1, 0, 0, 0.5843f) : new Color(1, 1, 1, 0.5843f);
    }

    /**************
     * Networking *
     **************/

    [Header("Networking")]
    public int tcpPort = 5127;
    public int udpPort = 5128;
    public string mainLevel = "Map";

    public void Play() {
        SaveSeed();
        TNManager.LoadLevel(mainLevel);
    }

    public void Connect(InputField ipInput) {
        Connect(ipInput.text);
    }

    public void Connect(string ip) {
        if (ip == null || ip.Length <= 0) return;

        TNAutoJoin autoJoin = tnManager.GetComponent<TNAutoJoin>();
        autoJoin.serverPort = tcpPort;
        autoJoin.firstLevel = mainLevel;
        autoJoin.serverAddress = ip;
        autoJoin.Connect();
    }

    public void CreateSession(){
        TNServerInstance.Start(5127, 5128);
        TNManager.LoadLevel(mainLevel);
    }

}
