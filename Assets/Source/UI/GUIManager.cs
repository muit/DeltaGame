using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
    void Start () {
	
	}
	
	void Update () {
	
	}

    public void Show(string name) {
        Transform uiChild = transform.FindChild(name);
        if (uiChild) {
            uiChild.gameObject.SetActive(true);
        } else {
            Debug.LogWarning("Couldnt find ui element " + name);
        }
    }

    public void Hide(string name)
    {
        Transform uiChild = transform.FindChild(name);
        if (uiChild) {
            uiChild.gameObject.SetActive(false);
        } else {
            Debug.LogWarning("Couldnt find ui element "+name);
        }
    }

    //Handlers
    public void Play() {
        Game.StartGame();
    }

    public void Pause()
    {
        Game.PauseGame();
    }
}
