using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour {

    public Text money;
    public Image HPBar;

    private RectTransform HPValue;
    private float maxWidth = 0;

	void Start () {
        HPValue = HPBar.GetComponentInChildren<RectTransform>();
        SetHP(0.5f);

        maxWidth = HPBar.GetComponent<RectTransform>().rect.width;
	}
	
	void Update () {
	
	}

    public void SetMoney(int amount) {
        money.text = amount.ToString();
    }

    public void SetHP(float percent)
    {
        float hpWidth = maxWidth * (1 - percent);
        HPValue.localPosition = new Vector3(hpWidth, HPValue.localPosition.y, HPValue.localPosition.z);
    }
}
