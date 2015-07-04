using UnityEngine;
using System.Collections;

public class HPBar : CanvasLookAtCamera{
    private RectTransform hpImage;
    private float maxWidth;

	protected override void Start () {
        base.Start();

        Transform imageBg = transform.FindChild("HPMask");
        Transform image = imageBg.transform.FindChild("HP");
        if (!hpImage) hpImage = image.GetComponent<RectTransform>();

        maxWidth = GetComponent<RectTransform>().rect.width;
	}

    public void SetHP(float percent)
    {
        float hpWidth = maxWidth * (1-percent);
        hpImage.localPosition = new Vector3(hpWidth, hpImage.localPosition.y, hpImage.localPosition.z);
        //hpImage.sizeDelta = new Vector2(hpWidth, hpImage.rect.height);
    }
}
