using UnityEngine;
using UnityEngine.UI;

public class AdrenalineBar : MonoBehaviour
{
    [SerializeField] Image ASlider;
    [SerializeField] RectTransform ASliderTransform, ABarTransform;
    Vector2 OrigBarPos;
    Vector2 OrigSliderPos;

    [SerializeField] Image SSlider;


    void Start()
    {
        OrigBarPos = ABarTransform.anchoredPosition;
        OrigSliderPos = ASliderTransform.anchoredPosition;
        PlayerStats.Stats.Event_AdrenalineValueChanged.AddListener((float adrenaline, float max) => ASlider.fillAmount = adrenaline/max);

        PlayerStats.Stats.Event_SecurityRaise.AddListener(IncreaseBarSize);
    }

    void IncreaseBarSize(float security, float adrenalineMax)
    {
        ASliderTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, adrenalineMax * 5);
        ABarTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, adrenalineMax * 5);

        SSlider.fillAmount = security / 200;

        Vector2 newBarPos = OrigBarPos;
        newBarPos.y = OrigBarPos.y + security * 2.5f;
        ABarTransform.anchoredPosition = newBarPos;

        Vector2 newSliderPos = OrigSliderPos;
        newSliderPos.y = OrigSliderPos.y + security * 2.5f;
        ASliderTransform.anchoredPosition = newSliderPos;
    }

  
    
}
