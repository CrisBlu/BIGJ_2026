using UnityEngine;
using UnityEngine.UI;

public class AdrenalineBar : MonoBehaviour
{
    [SerializeField] Image Slider;
    [SerializeField] RectTransform SliderTransform, BarTransform;
    Vector2 OrigBarPos;
    Vector2 OrigSliderPos;

    void Start()
    {
        OrigBarPos = BarTransform.anchoredPosition;
        OrigSliderPos = SliderTransform.anchoredPosition;
        PlayerStats.Stats.Event_AdrenalineValueChanged.AddListener((float adrenaline, float max) => Slider.fillAmount = adrenaline/max);

        PlayerStats.Stats.Event_SecurityRaise.AddListener(IncreaseBarSize);
    }

    void IncreaseBarSize(float security, float adrenalineMax)
    {
        SliderTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, adrenalineMax * 5);
        BarTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, adrenalineMax * 5);
        

        Vector2 newBarPos = OrigBarPos;
        newBarPos.y = OrigBarPos.y + security * 2.5f;
        BarTransform.anchoredPosition = newBarPos;

        Vector2 newSliderPos = OrigSliderPos;
        newSliderPos.y = OrigSliderPos.y + security * 2.5f;
        SliderTransform.anchoredPosition = newSliderPos;
    }

  
    
}
