using UnityEngine;
using UnityEngine.UI;

public class AdrenalineBar : MonoBehaviour
{
    [SerializeField] Image Slider;

    void Start()
    {
        PlayerStats.Stats.Event_AdrenalineValueChanged.AddListener((float adrenaline, float max) => Slider.fillAmount = adrenaline/max);
    }

  
    
}
