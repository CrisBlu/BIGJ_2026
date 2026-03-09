using UnityEngine;
using UnityEngine.UI;
public class GuardDetectionUI : MonoBehaviour
{
    public Slider timeSlider;
    public float increaseRate = 0.25f;
    public GameObject MainCharacter;
    public AISensor guardScript;

    private bool hasTurnedOff = false;

    void Start()
    {
        if (timeSlider != null)
        {
            timeSlider.value = timeSlider.minValue;
            timeSlider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (timeSlider == null || guardScript == null) return;

        bool inSight = guardScript.Objects.Contains(MainCharacter); // just check the list

        if (inSight)
        {
            timeSlider.gameObject.SetActive(true);
            timeSlider.value += increaseRate * Time.deltaTime;

            if (timeSlider.value >= timeSlider.maxValue && !hasTurnedOff)
            {
                timeSlider.value = timeSlider.maxValue;
                hasTurnedOff = true;
                timeSlider.gameObject.SetActive(false);
            }
        }
        else
        {
            timeSlider.value -= increaseRate * Time.deltaTime;

            if (timeSlider.value <= timeSlider.minValue)
            {
                timeSlider.value = timeSlider.minValue;
                timeSlider.gameObject.SetActive(false);
            }
        }
    }
}
