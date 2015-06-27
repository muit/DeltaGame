using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour {
    [Header("References")]
    public InputField seedInput;
    public Button joinButton;

    private TNManager tnManager;

	void Start () {
        tnManager = FindObjectOfType<TNManager>();
	}


	
	void Update () {
	}

    public void ShowSeed() {
        seedInput.text = FindObjectOfType<SaveLoad>().LoadSeed();
    }
    public void SaveSeed() {
        FindObjectOfType<SaveLoad>().SaveSeed(seedInput.text);
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
