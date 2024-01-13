using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameWaveTrackerItem : MonoBehaviour
{
    public TMPro.TextMeshProUGUI _tmpWave;
    public UnityEngine.UI.Image _imgIcon;
    public UnityEngine.UI.Image gArrow;

    public Color _colorIsIn;
    public void ParseData(int wave, Sprite icon, bool isLastWave)
    {
        gArrow.gameObject.SetActive(!isLastWave);
        this._tmpWave.text = wave.ToString();
        this._imgIcon.sprite = icon;
    }
    public void SetIsIn(bool isIn)
    {
        this._tmpWave.color = isIn ? _colorIsIn : Color.white;
        this.gArrow.color = isIn ? _colorIsIn : Color.white;
    }
}
