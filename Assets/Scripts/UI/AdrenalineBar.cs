using UnityEngine;
using UnityEngine.UI;

public class AdrenalineBar : MonoBehaviour
{
    [SerializeField] Image Slider;
    [SerializeField] RectTransform SliderTransform, BarTransform;

    void Start()
    {
        PlayerStats.Stats.Event_AdrenalineValueChanged.AddListener((float adrenaline, float max) => Slider.fillAmount = adrenaline/max);

        PlayerStats.Stats.Event_SecurityRaise.AddListener(IncreaseBarSize);
    }

    void IncreaseBarSize(float security, float adrenalineMax)
    {
        SliderTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, adrenalineMax * 5);
        BarTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, adrenalineMax * 5);
    }

  
    
}
